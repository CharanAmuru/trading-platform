using Microsoft.AspNetCore.Mvc;
using TradingPlatform.Application.Commands.PlaceOrder;
using TradingPlatform.Application.DTOs;

namespace TradingPlatform.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly PlaceOrderHandler _handler;

    public OrdersController(PlaceOrderHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("{accountId:guid}")]
    public async Task<ActionResult<OrderResponse>> PlaceOrder(
        [FromRoute] Guid accountId,
        [FromBody] PlaceOrderRequest body,
        CancellationToken ct)
    {
        if (body?.Request is null) return BadRequest("Body.request is required.");

        var cmd = new PlaceOrderCommand(accountId, body.Request);
        var result = await _handler.HandleAsync(cmd, ct);
        return Ok(result);
    }
}
