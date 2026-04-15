using Microsoft.EntityFrameworkCore;
using Pos.Modules.Catalog.Domain.Entities;

namespace Pos.Modules.Catalog.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("catalog");

        modelBuilder.Entity<Category>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Price).HasPrecision(18, 2);
            b.HasOne(x => x.Category).WithMany(c => c.Products).HasForeignKey(x => x.CategoryId);
        });
    }
}

