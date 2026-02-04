using TradingPlatform.Application.Interfaces;

namespace TradingPlatform.Infrastructure.Pricing;

public sealed class InMemoryMarketPriceProvider : IMarketPriceProvider
{
    public decimal GetPrice(Guid instrumentId)
    {
        // deterministic mock price per instrument (no DB needed)
        var n = Math.Abs(instrumentId.GetHashCode());
        var basePrice = (n % 9000) / 100m + 10m; // 10.00 to 99.99
        return decimal.Round(basePrice, 2);
    }
}
