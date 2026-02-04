using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Domain.Entities;
using TradingPlatform.Domain.Enums;
using TradingPlatform.Infrastructure.Persistence;

namespace TradingPlatform.Worker.Workers;

public sealed class ExecutionWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ExecutionWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
                var prices = scope.ServiceProvider.GetRequiredService<IMarketPriceProvider>();
                var positions = scope.ServiceProvider.GetRequiredService<IPositionRepository>();

                var job = await db.ExecutionJobs
                    .Where(x => x.Status == "Pending")
                    .OrderBy(x => x.CreatedAtUtc)
                    .FirstOrDefaultAsync(stoppingToken);

                if (job is null)
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                job.Status = "InProgress";
                job.LockedAtUtc = DateTime.UtcNow;
                await db.SaveChangesAsync(stoppingToken);

                try
                {
                    var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == job.OrderId, stoppingToken);
                    if (order is null)
                        throw new InvalidOperationException("Order not found for job.");

                    var fillQty = order.RemainingQuantity;

                    if (fillQty > 0m)
                    {
                        order.ApplyFill(fillQty);

                        var fillPrice = order.Type == OrderType.Limit
                            ? (order.LimitPrice ?? prices.GetPrice(order.InstrumentId))
                            : prices.GetPrice(order.InstrumentId);

                        var existing = await positions.GetAsync(order.AccountId, order.InstrumentId, stoppingToken);

                        var oldQty = existing?.Quantity ?? 0m;
                        var oldAvg = existing?.AvgPrice ?? 0m;

                        var newQty = oldQty + fillQty;

                        var newAvg = newQty == 0m
                            ? 0m
                            : ((oldQty * oldAvg) + (fillQty * fillPrice)) / newQty;

                        var updated = new Position(
                            accountId: order.AccountId,
                            instrumentId: order.InstrumentId,
                            quantity: newQty,
                            avgPrice: decimal.Round(newAvg, 6)
                        );

                        await positions.UpsertAsync(updated, stoppingToken);

                        await db.SaveChangesAsync(stoppingToken);
                    }

                    job.Status = "Completed";
                    job.LockedAtUtc = null;
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch
                {
                    job.Status = "Failed";
                    job.LockedAtUtc = null;
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
