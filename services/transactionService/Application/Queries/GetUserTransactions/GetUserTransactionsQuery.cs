using MediatR;
using SharedKernel.ResultPattern;
using transactionService.Domain.Enums;

namespace transactionService.Application.Queries.GetUserTransactions;

public class GetUserTransactionsQuery : IRequest<Result<IEnumerable<TransactionSummaryDto>>>
{
    public Guid UserId { get; set; }
    public TransactionType? Type { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
