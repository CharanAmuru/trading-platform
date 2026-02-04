using System.Collections.Concurrent;
using TradingPlatform.Application.Interfaces;

namespace TradingPlatform.Infrastructure.Queue;

public sealed class InMemoryExecutionQueue : IExecutionQueue, IExecutionQueueReader
{
    private readonly ConcurrentQueue<Guid> _queue = new();

    // IExecutionQueue (writer)
    public Task EnqueueAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        _queue.Enqueue(orderId);
        return Task.CompletedTask;
    }

    // IExecutionQueueReader (reader)
    public Task<Guid?> DequeueAsync(CancellationToken cancellationToken)
    {
        if (_queue.TryDequeue(out var orderId))
            return Task.FromResult<Guid?>(orderId);

        return Task.FromResult<Guid?>(null);
    }

    public Task MarkDoneAsync(Guid id, CancellationToken cancellationToken)
    {
        // In-memory queue: nothing to persist
        return Task.CompletedTask;
    }
}
