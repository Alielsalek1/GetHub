using MediatR;
using SharedKernel.ResultPattern;

namespace transactionService.Application.Queries.GetTransactionById;

public class GetTransactionByIdQuery : IRequest<Result<TransactionDetailsDto>>
{
    public Guid TransactionId { get; set; }
}
