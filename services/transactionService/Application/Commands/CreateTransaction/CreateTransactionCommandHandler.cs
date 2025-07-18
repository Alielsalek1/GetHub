using transactionService.Application.Commands.CreateTransaction;
using transactionService.Domain.Entities;
using transactionService.Domain.Events;
using transactionService.Domain.Interfaces;
using transactionService.Domain.Enums;
using MassTransit;
using MediatR;
using SharedKernel.ResultPattern;

namespace transactionService.Application.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Result<Guid>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IPublishEndpoint publishEndpoint)
    {
        _transactionRepository = transactionRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            if (request.Amount <= 0)
                return Result<Guid>.Failure("Amount must be greater than zero");

            if (!request.Items.Any())
                return Result<Guid>.Failure("Transaction must have at least one item");

            // Create transaction
            var transaction = new Transaction(
                request.UserId,
                request.Type,
                request.Amount,
                request.PaymentMethod,
                request.Description,
                request.Currency);

            // Add items to transaction
            foreach (var item in request.Items)
            {
                transaction.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
            }

            // Save transaction
            var createResult = await _transactionRepository.CreateAsync(transaction);
            if (!createResult.IsSuccess)
                return Result<Guid>.Failure($"Failed to create transaction: {createResult.Error}");

            // Publish transaction created event
            await _publishEndpoint.Publish(new TransactionCreated(
                transaction.Id,
                transaction.UserId,
                transaction.Type,
                transaction.Amount,
                transaction.Currency,
                transaction.PaymentMethod,
                transaction.ReferenceNumber!,
                transaction.CreatedAt
            ), cancellationToken);

            // Publish item-specific events
            foreach (var item in transaction.Items)
            {
                if (transaction.Type == TransactionType.Sale)
                {
                    await _publishEndpoint.Publish(new ItemSold(
                        transaction.Id,
                        transaction.UserId,
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.UnitPrice,
                        item.TotalPrice,
                        transaction.CreatedAt
                    ), cancellationToken);
                }
                else if (transaction.Type == TransactionType.Purchase)
                {
                    await _publishEndpoint.Publish(new ItemPurchased(
                        transaction.Id,
                        transaction.UserId,
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.UnitPrice,
                        item.TotalPrice,
                        transaction.CreatedAt
                    ), cancellationToken);
                }
            }

            return Result<Guid>.Success(transaction.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Error creating transaction: {ex.Message}");
        }
    }
}
