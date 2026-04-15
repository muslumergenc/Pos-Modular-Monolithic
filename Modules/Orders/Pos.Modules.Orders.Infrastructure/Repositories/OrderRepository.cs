using Microsoft.EntityFrameworkCore;
using Pos.Modules.Orders.Application.Interfaces;
using Pos.Modules.Orders.Domain.Entities;
using Pos.Modules.Orders.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Pos.Modules.Orders.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _context;
    public OrderRepository(OrdersDbContext context) => _context = context;

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Orders.Include(o => o.Items).ToListAsync(ct);

    public async Task<IEnumerable<Order>> FindAsync(Expression<Func<Order, bool>> predicate, CancellationToken ct = default) =>
        await _context.Orders.Include(o => o.Items).Where(predicate).ToListAsync(ct);

    public async Task AddAsync(Order entity, CancellationToken ct = default) =>
        await _context.Orders.AddAsync(entity, ct);

    public void Update(Order entity) => _context.Orders.Update(entity);
    public void Delete(Order entity) => _context.Orders.Remove(entity);
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);
}

