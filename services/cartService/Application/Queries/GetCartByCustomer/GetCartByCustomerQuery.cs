using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Queries.GetCartByCustomer;

public class GetCartByCustomerQuery : IRequest<Result<CartDetailsDto>>
{
    public Guid CustomerId { get; set; }
}
