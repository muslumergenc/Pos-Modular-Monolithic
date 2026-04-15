using MediatR;
using Pos.Modules.Orders.Application.DTOs;
using Pos.Shared.Common;

namespace Pos.Modules.Orders.Application.Queries;

public record GetAllOrdersQuery(Guid? CustomerId = null) : IRequest<Result<IEnumerable<OrderDto>>>;
public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;

