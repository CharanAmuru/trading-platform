namespace TradingPlatform.Application.Interfaces;

public interface IExecutionQueueReader
{
    Task<Guid?> DequeueAsync(CancellationToken ct);
    Task MarkDoneAsync(Guid orderId, CancellationToken ct);
}
