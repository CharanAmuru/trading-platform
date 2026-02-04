using TradingPlatform.Application.DTOs;

namespace TradingPlatform.Application.Commands.PlaceOrder;

public sealed record PlaceOrderCommand(Guid AccountId, CreateOrderRequest Request);
