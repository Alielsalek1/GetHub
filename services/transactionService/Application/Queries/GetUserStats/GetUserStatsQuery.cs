using MediatR;
using SharedKernel.ResultPattern;

namespace transactionService.Application.Queries.GetUserStats;

public class GetUserStatsQuery : IRequest<Result<UserTransactionStatsDto>>
{
    public Guid UserId { get; set; }
}
