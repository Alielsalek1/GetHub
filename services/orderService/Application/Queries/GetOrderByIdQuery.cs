using MediatR;
using FluentResults;

namespace orderService.Application.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderReadModel>>;

public class OrderReadModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public List<OrderItemReadModel> Items { get; set; } = new();
}

public class OrderItemReadModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
