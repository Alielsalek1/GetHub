using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Commands.UpdateItemQuantity;

public class UpdateItemQuantityCommand : IRequest<Result<bool>>
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public int NewQuantity { get; set; }
}
