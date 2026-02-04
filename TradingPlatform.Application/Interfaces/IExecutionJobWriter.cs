namespace TradingPlatform.Application.Interfaces;

public interface IExecutionJobWriter
{
    Task EnqueueAsync(Guid orderId, CancellationToken ct);
}
