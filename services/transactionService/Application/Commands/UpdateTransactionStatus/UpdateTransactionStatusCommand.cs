using MediatR;
using SharedKernel.ResultPattern;
using transactionService.Domain.Enums;

namespace transactionService.Application.Commands.UpdateTransactionStatus;

public class UpdateTransactionStatusCommand : IRequest<Result<bool>>
{
    public Guid TransactionId { get; set; }
    public TransactionStatus NewStatus { get; set; }
    public string? Reason { get; set; }
}
