using Microsoft.EntityFrameworkCore;
using Pos.Modules.Customers.Domain.Entities;

namespace Pos.Modules.Customers.Infrastructure.Persistence;

public class CustomersDbContext : DbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options) { }
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("customers");
        modelBuilder.Entity<Customer>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            b.Property(x => x.Email).IsRequired().HasMaxLength(200);
            b.Property(x => x.Phone).IsRequired().HasMaxLength(20);
        });
    }
}

