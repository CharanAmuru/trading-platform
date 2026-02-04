using TradingPlatform.Domain.Enums;
using TradingPlatform.Domain.Exceptions;

namespace TradingPlatform.Domain.Entities;

public sealed class Order
{

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AccountId { get; private set; }
    public Guid InstrumentId { get; private set; }

    public OrderSide Side { get; private set; }
    public OrderType Type { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.New;

    public decimal Quantity { get; private set; }
    public decimal FilledQuantity { get; private set; }
    public decimal? LimitPrice { get; private set; }

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

    private Order() { } // for EF Core later (safe to keep)

    public Order(
        Guid accountId,
        Guid instrumentId,
        OrderSide side,
        OrderType type,
        decimal quantity,
        decimal? limitPrice)
    {
        if (accountId == Guid.Empty) throw new DomainException("AccountId is required.");
        if (instrumentId == Guid.Empty) throw new DomainException("InstrumentId is required.");
        if (quantity <= 0) throw new DomainException("Quantity must be > 0.");

        if (type == OrderType.Limit)
        {
            if (limitPrice is null || limitPrice <= 0)
                throw new DomainException("LimitPrice must be > 0 for limit orders.");
        }
        else
        {
            if (limitPrice is not null)
                throw new DomainException("LimitPrice must be null for market orders.");
        }

        AccountId = accountId;
        InstrumentId = instrumentId;
        Side = side;
        Type = type;
        Quantity = quantity;
        LimitPrice = limitPrice;
    }

    public decimal RemainingQuantity => Quantity - FilledQuantity;

    public void Accept()
    {
        EnsureStatus(OrderStatus.New);
        Status = OrderStatus.Accepted;
        Touch();
    }

    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Reject reason is required.");

        if (Status is OrderStatus.Filled or OrderStatus.Canceled)
            throw new DomainException("Cannot reject a filled or canceled order.");

        Status = OrderStatus.Rejected;
        Touch();
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Filled or OrderStatus.Canceled or OrderStatus.Rejected)
            throw new DomainException("Order cannot be canceled in its current state.");

        Status = OrderStatus.Canceled;
        Touch();
    }

    public void ApplyFill(decimal fillQuantity)
    {
        if (fillQuantity <= 0) throw new DomainException("Fill quantity must be > 0.");

        if (Status is OrderStatus.Canceled or OrderStatus.Rejected)
            throw new DomainException("Cannot fill a canceled or rejected order.");
    
        if (FilledQuantity + fillQuantity > Quantity)
            throw new DomainException("Fill exceeds order quantity.");

        FilledQuantity += fillQuantity;

        if (FilledQuantity == Quantity)
            Status = OrderStatus.Filled;
        else
            Status = OrderStatus.PartiallyFilled;

        Touch();
    }

    private void EnsureStatus(OrderStatus expected)
    {
        if (Status != expected)
            throw new DomainException($"Expected status {expected} but was {Status}.");
    }

    private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}
