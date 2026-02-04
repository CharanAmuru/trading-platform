using TradingPlatform.Domain.Enums;

namespace TradingPlatform.Application.DTOs;

public sealed record OrderResponse(
    Guid OrderId,
    OrderStatus Status,
    decimal Quantity,
    decimal FilledQuantity,
    decimal RemainingQuantity
);
