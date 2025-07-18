using MediatR;
using SharedKernel.ResultPattern;
using transactionService.Domain.Enums;

namespace transactionService.Application.Commands.CreateTransaction;

public class CreateTransactionCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentMethod PaymentMethod { get; set; }
    public string? Description { get; set; }
    public List<TransactionItemDto> Items { get; set; } = new();
}

public class TransactionItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
