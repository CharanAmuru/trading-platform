using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);

    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<Order>> ListAsync(
        int skip,
        int take,
        CancellationToken ct);
}
