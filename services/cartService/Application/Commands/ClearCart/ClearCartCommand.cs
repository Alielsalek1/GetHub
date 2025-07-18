using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Commands.ClearCart;

public class ClearCartCommand : IRequest<Result<bool>>
{
    public Guid CustomerId { get; set; }
}
