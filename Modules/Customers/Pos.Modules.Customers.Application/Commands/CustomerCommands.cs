using MediatR;
using Pos.Modules.Customers.Application.DTOs;
using Pos.Shared.Common;

namespace Pos.Modules.Customers.Application.Commands;

public record CreateCustomerCommand(string FullName, string Email, string Phone, string? Address) : IRequest<Result<CustomerDto>>;
public record UpdateCustomerCommand(Guid Id, string FullName, string Email, string Phone, string? Address) : IRequest<Result<CustomerDto>>;
public record DeleteCustomerCommand(Guid Id) : IRequest<Result>;

