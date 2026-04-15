using Pos.Modules.Orders.Domain.Enums;

namespace Pos.Modules.Orders.Application.DTOs;

public record OrderItemDto(Guid Id, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice);
public record OrderDto(Guid Id, Guid CustomerId, string CustomerName, OrderStatus Status, decimal TotalAmount, string? Notes, IEnumerable<OrderItemDto> Items, DateTime CreatedAt);
public record CreateOrderItemDto(Guid ProductId, int Quantity);
public record CreateOrderDto(Guid CustomerId, string? Notes, IEnumerable<CreateOrderItemDto> Items);
public record UpdateOrderStatusDto(OrderStatus Status);

