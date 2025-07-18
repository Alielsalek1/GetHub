using MediatR;
using FluentResults;

namespace orderService.Application.Queries;

public record GetOrdersByCustomerQuery(Guid CustomerId) : IRequest<Result<List<OrderSummaryReadModel>>>;

public class OrderSummaryReadModel
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}
