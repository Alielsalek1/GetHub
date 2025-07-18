using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Commands.RemoveItemFromCart;

public class RemoveItemFromCartCommand : IRequest<Result<bool>>
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
}
