using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Application.Interfaces;

public interface IPositionRepository
{
    Task<Position?> GetAsync(Guid accountId, Guid instrumentId, CancellationToken ct);
    Task<IReadOnlyList<Position>> GetByAccountAsync(Guid accountId, CancellationToken ct);
    Task UpsertAsync(Position position, CancellationToken ct);
}
