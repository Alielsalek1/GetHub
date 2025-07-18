using transactionService.Application.Commands.UpdateTransactionStatus;
using transactionService.Domain.Events;
using transactionService.Domain.Interfaces;
using transactionService.Domain.Enums;
using MassTransit;
using MediatR;
using SharedKernel.ResultPattern;

namespace transactionService.Application.Commands.UpdateTransactionStatus;

public class UpdateTransactionStatusCommandHandler : IRequestHandler<UpdateTransactionStatusCommand, Result<bool>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateTransactionStatusCommandHandler(ITransactionRepository transactionRepository, IPublishEndpoint publishEndpoint)
    {
        _transactionRepository = transactionRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<bool>> Handle(UpdateTransactionStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get transaction
            var transactionResult = await _transactionRepository.GetByIdAsync(request.TransactionId);
            if (!transactionResult.IsSuccess)
                return Result<bool>.Failure("Transaction not found");

            var transaction = transactionResult.Value!;

            // Update status based on new status
            switch (request.NewStatus)
            {
                case TransactionStatus.Completed:
                    transaction.Complete();
                    await _publishEndpoint.Publish(new TransactionCompleted(
                        transaction.Id,
                        transaction.UserId,
                        transaction.Type,
                        transaction.Amount,
                        transaction.Currency,
                        transaction.CompletedAt!.Value,
                        transaction.ReferenceNumber!
                    ), cancellationToken);
                    break;

                case TransactionStatus.Cancelled:
                    transaction.Cancel(request.Reason);
                    await _publishEndpoint.Publish(new TransactionCancelled(
                        transaction.Id,
                        transaction.UserId,
                        transaction.Type,
                        transaction.Amount,
                        request.Reason ?? "No reason provided",
                        transaction.UpdatedAt
                    ), cancellationToken);
                    break;

                case TransactionStatus.Failed:
                    transaction.Fail(request.Reason);
                    await _publishEndpoint.Publish(new TransactionFailed(
                        transaction.Id,
                        transaction.UserId,
                        transaction.Type,
                        transaction.Amount,
                        request.Reason ?? "No reason provided",
                        transaction.UpdatedAt
                    ), cancellationToken);
                    break;

                default:
                    transaction.UpdateStatus(request.NewStatus);
                    break;
            }

            // Save transaction
            var updateResult = await _transactionRepository.UpdateAsync(transaction);
            if (!updateResult.IsSuccess)
                return Result<bool>.Failure($"Failed to update transaction: {updateResult.Error}");

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error updating transaction status: {ex.Message}");
        }
    }
}
