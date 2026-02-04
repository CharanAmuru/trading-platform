using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Domain.Entities;
using TradingPlatform.Infrastructure.Persistence;

namespace TradingPlatform.Infrastructure.Repositories;

public sealed class PositionRepository : IPositionRepository
{
    private readonly TradingDbContext _db;

    public PositionRepository(TradingDbContext db)
    {
        _db = db;
    }

    public Task<Position?> GetAsync(Guid accountId, Guid instrumentId, CancellationToken ct)
    {
        return _db.Positions
            .FirstOrDefaultAsync(p => p.AccountId == accountId && p.InstrumentId == instrumentId, ct);
    }

    public async Task<IReadOnlyList<Position>> GetByAccountAsync(Guid accountId, CancellationToken ct)
    {
        return await _db.Positions
            .Where(p => p.AccountId == accountId)
            .OrderByDescending(p => p.UpdatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task UpsertAsync(Position position, CancellationToken ct)
    {
        var existing = await _db.Positions
            .FirstOrDefaultAsync(p => p.AccountId == position.AccountId && p.InstrumentId == position.InstrumentId, ct);

        if (existing is null)
        {
            _db.Positions.Add(position);
        }
        else
        {
            existing.Set(position.Quantity, position.AvgPrice);
        }

        await _db.SaveChangesAsync(ct);
    }
}
