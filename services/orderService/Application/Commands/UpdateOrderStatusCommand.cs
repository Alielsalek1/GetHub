using MediatR;
using FluentResults;
using orderService.Domain.Entities;

namespace orderService.Application.Commands;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus
) : IRequest<Result>;
