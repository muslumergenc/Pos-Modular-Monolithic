using Microsoft.EntityFrameworkCore;
using Pos.Modules.Payments.Application.Interfaces;
using Pos.Modules.Payments.Domain.Entities;
using Pos.Modules.Payments.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Pos.Modules.Payments.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentsDbContext _context;
    public PaymentRepository(PaymentsDbContext context) => _context = context;

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Payments.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Payment>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Payments.ToListAsync(ct);

    public async Task<IEnumerable<Payment>> FindAsync(Expression<Func<Payment, bool>> predicate, CancellationToken ct = default) =>
        await _context.Payments.Where(predicate).ToListAsync(ct);

    public async Task AddAsync(Payment entity, CancellationToken ct = default) =>
        await _context.Payments.AddAsync(entity, ct);

    public void Update(Payment entity) => _context.Payments.Update(entity);
    public void Delete(Payment entity) => _context.Payments.Remove(entity);
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);
}

