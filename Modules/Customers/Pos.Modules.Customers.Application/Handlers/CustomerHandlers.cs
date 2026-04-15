using MediatR;
using Pos.Modules.Customers.Application.Commands;
using Pos.Modules.Customers.Application.DTOs;
using Pos.Modules.Customers.Application.Interfaces;
using Pos.Modules.Customers.Application.Queries;
using Pos.Modules.Customers.Domain.Entities;
using Pos.Shared.Common;

namespace Pos.Modules.Customers.Application.Handlers;

public class CustomerCommandHandler :
    IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>,
    IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>,
    IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly ICustomerRepository _repo;
    public CustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand req, CancellationToken ct)
    {
        var customer = Customer.Create(req.FullName, req.Email, req.Phone, req.Address);
        await _repo.AddAsync(customer, ct);
        await _repo.SaveChangesAsync(ct);
        return Result<CustomerDto>.Success(Map(customer));
    }

    public async Task<Result<CustomerDto>> Handle(UpdateCustomerCommand req, CancellationToken ct)
    {
        var customer = await _repo.GetByIdAsync(req.Id, ct);
        if (customer is null) return Result<CustomerDto>.Failure("Müşteri bulunamadı.");
        customer.Update(req.FullName, req.Email, req.Phone, req.Address);
        _repo.Update(customer);
        await _repo.SaveChangesAsync(ct);
        return Result<CustomerDto>.Success(Map(customer));
    }

    public async Task<Result> Handle(DeleteCustomerCommand req, CancellationToken ct)
    {
        var customer = await _repo.GetByIdAsync(req.Id, ct);
        if (customer is null) return Result.Failure("Müşteri bulunamadı.");
        _repo.Delete(customer);
        await _repo.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static CustomerDto Map(Customer c) => new(c.Id, c.FullName, c.Email, c.Phone, c.Address, c.IsActive, c.CreatedAt);
}

public class CustomerQueryHandler :
    IRequestHandler<GetAllCustomersQuery, Result<IEnumerable<CustomerDto>>>,
    IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerRepository _repo;
    public CustomerQueryHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<CustomerDto>>> Handle(GetAllCustomersQuery req, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(ct);
        return Result<IEnumerable<CustomerDto>>.Success(list.Select(c => new CustomerDto(c.Id, c.FullName, c.Email, c.Phone, c.Address, c.IsActive, c.CreatedAt)));
    }

    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery req, CancellationToken ct)
    {
        var c = await _repo.GetByIdAsync(req.Id, ct);
        if (c is null) return Result<CustomerDto>.Failure("Müşteri bulunamadı.");
        return Result<CustomerDto>.Success(new CustomerDto(c.Id, c.FullName, c.Email, c.Phone, c.Address, c.IsActive, c.CreatedAt));
    }
}

