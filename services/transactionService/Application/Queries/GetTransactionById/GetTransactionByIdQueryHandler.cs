using transactionService.Application.Queries.GetTransactionById;
using transactionService.Application.Queries.Interfaces;
using transactionService.Application.Queries.Models;
using MediatR;
using SharedKernel.ResultPattern;

namespace transactionService.Application.Queries.GetTransactionById;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Result<TransactionDetailsDto>>
{
    private readonly ITransactionQueryRepository _transactionQueryRepository;

    public GetTransactionByIdQueryHandler(ITransactionQueryRepository transactionQueryRepository)
    {
        _transactionQueryRepository = transactionQueryRepository;
    }

    public async Task<Result<TransactionDetailsDto>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _transactionQueryRepository.GetTransactionDetailsAsync(request.TransactionId);
        }
        catch (Exception ex)
        {
            return Result<TransactionDetailsDto>.Failure($"Error retrieving transaction: {ex.Message}");
        }
    }
}
