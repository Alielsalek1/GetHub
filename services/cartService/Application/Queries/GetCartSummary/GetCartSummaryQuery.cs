using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Queries.GetCartSummary;

public class GetCartSummaryQuery : IRequest<Result<CartSummaryDto>>
{
    public Guid CustomerId { get; set; }
}
