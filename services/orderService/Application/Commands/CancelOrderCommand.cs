using MediatR;
using FluentResults;

namespace orderService.Application.Commands;

public record CancelOrderCommand(
    Guid OrderId
) : IRequest<Result>;
