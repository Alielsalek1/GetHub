using transactionService.Application.Queries.GetUserTransactions;
using transactionService.Application.Queries.Interfaces;
using transactionService.Application.Queries.Models;
using MediatR;
using SharedKernel.ResultPattern;

namespace transactionService.Application.Queries.GetUserTransactions;

public class GetUserTransactionsQueryHandler : IRequestHandler<GetUserTransactionsQuery, Result<IEnumerable<TransactionSummaryDto>>>
{
    private readonly ITransactionQueryRepository _transactionQueryRepository;

    public GetUserTransactionsQueryHandler(ITransactionQueryRepository transactionQueryRepository)
    {
        _transactionQueryRepository = transactionQueryRepository;
    }

    public async Task<Result<IEnumerable<TransactionSummaryDto>>> Handle(GetUserTransactionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Type.HasValue)
            {
                return await _transactionQueryRepository.GetUserTransactionsByTypeAsync(
                    request.UserId, 
                    request.Type.Value, 
                    request.PageNumber, 
                    request.PageSize);
            }

            return await _transactionQueryRepository.GetUserTransactionsAsync(
                request.UserId, 
                request.PageNumber, 
                request.PageSize);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TransactionSummaryDto>>.Failure($"Error retrieving user transactions: {ex.Message}");
        }
    }
}
