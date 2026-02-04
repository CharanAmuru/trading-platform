using Microsoft.EntityFrameworkCore;
using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Infrastructure.Persistence;

public sealed class TradingDbContext : DbContext
{
    public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<ExecutionJob> ExecutionJobs => Set<ExecutionJob>();
    public DbSet<Position> Positions => Set<Position>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.Quantity).HasPrecision(18, 2);
            b.Property(x => x.FilledQuantity).HasPrecision(18, 2);
            b.Property(x => x.LimitPrice).HasPrecision(18, 2);

            b.HasIndex(x => new { x.AccountId, x.CreatedAtUtc });
            b.HasIndex(x => new { x.Status, x.UpdatedAtUtc });
        });
        modelBuilder.Entity<Order>(b =>
        {
            b.Property(x => x.Quantity).HasPrecision(18, 6);
            b.Property(x => x.FilledQuantity).HasPrecision(18, 6);
            b.Property(x => x.LimitPrice).HasPrecision(18, 6);
        });

        modelBuilder.Entity<Position>(b =>
        {
            b.Property(x => x.Quantity).HasPrecision(18, 6);
            b.Property(x => x.AvgPrice).HasPrecision(18, 6);
        });

        modelBuilder.Entity<ExecutionJob>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.CreatedAtUtc);
        });

        modelBuilder.Entity<Position>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.Quantity).HasPrecision(18, 2);
            b.Property(x => x.AvgPrice).HasPrecision(18, 2);

            b.HasIndex(x => new { x.AccountId, x.InstrumentId }).IsUnique();
        });
    }
}
