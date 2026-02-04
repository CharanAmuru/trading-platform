using TradingPlatform.Domain.Exceptions;

namespace TradingPlatform.Domain.Entities;

public sealed class Position
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid AccountId { get; private set; }
    public Guid InstrumentId { get; private set; }

    public decimal Quantity { get; private set; }
    public decimal AvgPrice { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

    private Position() { } // EF Core

    public Position(Guid accountId, Guid instrumentId, decimal quantity, decimal avgPrice)
    {
        if (accountId == Guid.Empty) throw new DomainException("AccountId is required.");
        if (instrumentId == Guid.Empty) throw new DomainException("InstrumentId is required.");
        if (quantity < 0) throw new DomainException("Position quantity cannot be negative.");
        if (avgPrice < 0) throw new DomainException("AvgPrice cannot be negative.");

        AccountId = accountId;
        InstrumentId = instrumentId;
        Quantity = quantity;
        AvgPrice = avgPrice;
        Touch();
    }

    public void Set(decimal quantity, decimal avgPrice)
    {
        if (quantity < 0) throw new DomainException("Position quantity cannot be negative.");
        if (avgPrice < 0) throw new DomainException("AvgPrice cannot be negative.");

        Quantity = quantity;
        AvgPrice = avgPrice;
        Touch();
    }

    private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}
