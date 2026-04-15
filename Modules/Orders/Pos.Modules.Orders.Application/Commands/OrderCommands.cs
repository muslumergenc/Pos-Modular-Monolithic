using MediatR;
using Pos.Modules.Orders.Application.DTOs;
using Pos.Modules.Orders.Domain.Enums;
using Pos.Shared.Common;

namespace Pos.Modules.Orders.Application.Commands;

public record CreateOrderCommand(Guid CustomerId, string? Notes, IEnumerable<CreateOrderItemDto> Items) : IRequest<Result<OrderDto>>;
public record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<Result<OrderDto>>;
public record CancelOrderCommand(Guid Id) : IRequest<Result>;

