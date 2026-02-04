using TradingPlatform.Application.Interfaces;
using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Infrastructure.Persistence;

public sealed class SqlExecutionJobWriter : IExecutionJobWriter
{
    private readonly TradingDbContext _db;

    public SqlExecutionJobWriter(TradingDbContext db)
    {
        _db = db;
    }

    public async Task EnqueueAsync(Guid orderId, CancellationToken ct)
    {
        var job = new ExecutionJob
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Status = "Pending",
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.ExecutionJobs.Add(job);
        await _db.SaveChangesAsync(ct);
    }
}
