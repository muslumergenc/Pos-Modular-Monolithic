using Microsoft.EntityFrameworkCore;
using Pos.Modules.Catalog.Application.Interfaces;
using Pos.Modules.Catalog.Domain.Entities;
using Pos.Modules.Catalog.Infrastructure.Persistence;
using Pos.Shared.Domain;
using System.Linq.Expressions;

namespace Pos.Modules.Catalog.Infrastructure.Repositories;

public class BaseRepository<T> where T : BaseEntity
{
    protected readonly CatalogDbContext _context;
    public BaseRepository(CatalogDbContext context) => _context = context;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Set<T>().FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Set<T>().ToListAsync(ct);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _context.Set<T>().Where(predicate).ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default) =>
        await _context.Set<T>().AddAsync(entity, ct);

    public void Update(T entity) => _context.Set<T>().Update(entity);
    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(CatalogDbContext context) : base(context) { }

    public new async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id, ct);

    public new async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Products.Include(p => p.Category).ToListAsync(ct);

    public new async Task<IEnumerable<Product>> FindAsync(Expression<Func<Product, bool>> predicate, CancellationToken ct = default) =>
        await _context.Products.Include(p => p.Category).Where(predicate).ToListAsync(ct);
}

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(CatalogDbContext context) : base(context) { }
}

