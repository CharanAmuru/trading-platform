namespace TradingPlatform.Application.Interfaces;

public interface IExecutionQueue
{
    Task EnqueueAsync(Guid orderId, CancellationToken ct);
}
