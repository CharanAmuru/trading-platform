using TradingPlatform.Application.Interfaces;
using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Worker.Services;

public sealed class PositionService
{
    private readonly IPositionRepository _positions;

    public PositionService(IPositionRepository positions)
    {
        _positions = positions;
    }

    public async Task ApplyFillAsync(
        Guid accountId,
        Guid instrumentId,
        decimal fillQty,
        decimal fillPrice,
        CancellationToken ct)
    {
        // fetch existing position
        var existing = await _positions.GetAsync(accountId, instrumentId, ct);

        if (existing is null)
        {
            // IMPORTANT: use constructor (no property setters)
            var created = new Position(
                accountId: accountId,
                instrumentId: instrumentId,
                quantity: fillQty,
                avgPrice: decimal.Round(fillPrice, 6)
            );

            await _positions.UpsertAsync(created, ct);
            return;
        }

        // compute new qty + avg (no property setters)
        var oldQty = existing.Quantity;
        var oldAvg = existing.AvgPrice;

        var newQty = oldQty + fillQty;

        var newAvg = newQty == 0m
            ? 0m
            : ((oldQty * oldAvg) + (fillQty * fillPrice)) / newQty;

        var updated = new Position(
            accountId: accountId,
            instrumentId: instrumentId,
            quantity: newQty,
            avgPrice: decimal.Round(newAvg, 6)
        );

        await _positions.UpsertAsync(updated, ct);
    }

    public Task<IReadOnlyList<Position>> GetByAccountAsync(Guid accountId, CancellationToken ct)
        => _positions.GetByAccountAsync(accountId, ct);
}
