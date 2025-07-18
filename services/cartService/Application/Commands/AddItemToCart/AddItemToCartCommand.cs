using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Commands.AddItemToCart;

public class AddItemToCartCommand : IRequest<Result<bool>>
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
