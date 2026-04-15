using Microsoft.EntityFrameworkCore;
using Pos.Modules.Orders.Domain.Entities;

namespace Pos.Modules.Orders.Infrastructure.Persistence;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("orders");

        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.CustomerName).IsRequired().HasMaxLength(150);
            b.Property(x => x.TotalAmount).HasPrecision(18, 2);
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<OrderItem>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
        });
    }
}

