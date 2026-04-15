using Microsoft.EntityFrameworkCore;
using Pos.Modules.Customers.Application.Interfaces;
using Pos.Modules.Customers.Domain.Entities;
using Pos.Modules.Customers.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Pos.Modules.Customers.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomersDbContext _context;
    public CustomerRepository(CustomersDbContext context) => _context = context;

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Customers.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Customers.ToListAsync(ct);

    public async Task<IEnumerable<Customer>> FindAsync(Expression<Func<Customer, bool>> predicate, CancellationToken ct = default) =>
        await _context.Customers.Where(predicate).ToListAsync(ct);

    public async Task AddAsync(Customer entity, CancellationToken ct = default) =>
        await _context.Customers.AddAsync(entity, ct);

    public void Update(Customer entity) => _context.Customers.Update(entity);
    public void Delete(Customer entity) => _context.Customers.Remove(entity);
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);
}

