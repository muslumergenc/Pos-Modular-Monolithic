using Microsoft.EntityFrameworkCore;
using Pos.Modules.Tables.Domain.Entities;

namespace Pos.Modules.Tables.Infrastructure.Persistence;

public class TablesDbContext : DbContext
{
    public TablesDbContext(DbContextOptions<TablesDbContext> options) : base(options) { }
    public DbSet<RestaurantTable> Tables => Set<RestaurantTable>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("tables");

        modelBuilder.Entity<RestaurantTable>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Number).IsRequired();
            b.HasIndex(x => x.Number).IsUnique();
            b.Property(x => x.Label).HasMaxLength(100);
            b.Property(x => x.Capacity).IsRequired();
            b.Property(x => x.Status).IsRequired();
        });
    }
}
