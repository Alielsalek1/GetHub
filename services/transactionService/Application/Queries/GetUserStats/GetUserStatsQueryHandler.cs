using transactionService.Application.Queries.GetUserStats;
using transactionService.Application.Queries.Interfaces;
using transactionService.Application.Queries.Models;
using MediatR;
using SharedKernel.ResultPattern;

namespace transactionService.Application.Queries.GetUserStats;

public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, Result<UserTransactionStatsDto>>
{
    private readonly ITransactionQueryRepository _transactionQueryRepository;

    public GetUserStatsQueryHandler(ITransactionQueryRepository transactionQueryRepository)
    {
        _transactionQueryRepository = transactionQueryRepository;
    }

    public async Task<Result<UserTransactionStatsDto>> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _transactionQueryRepository.GetUserTransactionStatsAsync(request.UserId);
        }
        catch (Exception ex)
        {
            return Result<UserTransactionStatsDto>.Failure($"Error retrieving user transaction stats: {ex.Message}");
        }
    }
}
