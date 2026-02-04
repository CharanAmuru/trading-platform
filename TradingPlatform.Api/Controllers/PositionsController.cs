using Microsoft.AspNetCore.Mvc;
using TradingPlatform.Application.Interfaces;

namespace TradingPlatform.Api.Controllers;

[ApiController]
[Route("api/positions")]
public sealed class PositionsController : ControllerBase
{
    private readonly IPositionRepository _positions;
    private readonly IMarketPriceProvider _prices;

    public PositionsController(IPositionRepository positions, IMarketPriceProvider prices)
    {
        _positions = positions;
        _prices = prices;
    }

    [HttpGet("{accountId:guid}")]
    public async Task<IActionResult> GetPositions(Guid accountId, CancellationToken ct)
    {
        var list = await _positions.GetByAccountAsync(accountId, ct);

        var result = list.Select(p =>
        {
            var mkt = _prices.GetPrice(p.InstrumentId);
            var unrealized = (mkt - p.AvgPrice) * p.Quantity;

            return new
            {
                p.AccountId,
                p.InstrumentId,
                Quantity = p.Quantity,
                AvgPrice = p.AvgPrice,
                MarketPrice = mkt,
                UnrealizedPnl = decimal.Round(unrealized, 2),
                p.UpdatedAtUtc
            };
        });

        return Ok(result);
    }
}
