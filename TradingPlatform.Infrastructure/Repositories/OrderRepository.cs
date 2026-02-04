using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Domain.Entities;
using TradingPlatform.Infrastructure.Persistence;

namespace TradingPlatform.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly TradingDbContext _db;

    public OrderRepository(TradingDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _db.Orders.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Order>> ListAsync(int skip, int take, CancellationToken ct)
    {
        return await _db.Orders
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }
}
