namespace TradingPlatform.Application.Interfaces;

public interface IMarketPriceProvider
{
    decimal GetPrice(Guid instrumentId);
}
