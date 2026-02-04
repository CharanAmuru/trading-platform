using TradingPlatform.Domain.Enums;

namespace TradingPlatform.Application.DTOs;

public sealed record OrderDetailsResponse(
    Guid OrderId,
    Guid AccountId,
    Guid InstrumentId,
    OrderSide Side,
    OrderType Type,
    OrderStatus Status,
    decimal Quantity,
    decimal FilledQuantity,
    decimal RemainingQuantity,
    decimal? LimitPrice,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);
