using MediatR;
using Pos.Modules.Customers.Application.DTOs;
using Pos.Shared.Common;

namespace Pos.Modules.Customers.Application.Queries;

public record GetAllCustomersQuery : IRequest<Result<IEnumerable<CustomerDto>>>;
public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;

