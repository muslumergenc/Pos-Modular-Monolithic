using Microsoft.EntityFrameworkCore;
using Pos.Modules.Payments.Domain.Entities;

namespace Pos.Modules.Payments.Infrastructure.Persistence;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.Entity<Payment>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.Method).HasConversion<string>();
            b.Property(x => x.Status).HasConversion<string>();
        });
    }
}

