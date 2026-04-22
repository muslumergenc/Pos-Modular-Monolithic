using Microsoft.EntityFrameworkCore;
using Pos.Modules.Tables.Application.Interfaces;
using Pos.Modules.Tables.Domain.Entities;
using Pos.Modules.Tables.Infrastructure.Persistence;

namespace Pos.Modules.Tables.Infrastructure.Repositories;

public class TableRepository : ITableRepository
{
    private readonly TablesDbContext _ctx;
    public TableRepository(TablesDbContext ctx) => _ctx = ctx;

    public async Task<RestaurantTable?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _ctx.Tables.FindAsync([id], ct);

    public async Task<IEnumerable<RestaurantTable>> GetAllAsync(CancellationToken ct = default) =>
        await _ctx.Tables.OrderBy(t => t.Number).ToListAsync(ct);

    public async Task<IEnumerable<RestaurantTable>> FindAsync(System.Linq.Expressions.Expression<Func<RestaurantTable, bool>> predicate, CancellationToken ct = default) =>
        await _ctx.Tables.Where(predicate).ToListAsync(ct);

    public async Task<RestaurantTable?> GetByNumberAsync(int number, CancellationToken ct = default) =>
        await _ctx.Tables.FirstOrDefaultAsync(t => t.Number == number, ct);

    public async Task AddAsync(RestaurantTable entity, CancellationToken ct = default) =>
        await _ctx.Tables.AddAsync(entity, ct);

    public void Update(RestaurantTable entity) => _ctx.Tables.Update(entity);
    public void Delete(RestaurantTable entity) => _ctx.Tables.Remove(entity);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _ctx.SaveChangesAsync(ct);
}
