using TradingPlatform.Application.DTOs;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Application.Commands.PlaceOrder;

public sealed class PlaceOrderHandler
{
    private readonly IOrderRepository _orders;
    private readonly IExecutionJobWriter _jobs;

    public PlaceOrderHandler(IOrderRepository orders, IExecutionJobWriter jobs)
    {
        _orders = orders;
        _jobs = jobs;
    }

    public async Task<OrderResponse> HandleAsync(PlaceOrderCommand cmd, CancellationToken ct)
    {
        if (cmd is null) throw new ArgumentNullException(nameof(cmd));
        if (cmd.Request is null) throw new ArgumentNullException(nameof(cmd.Request));

        var r = cmd.Request;

        var order = new Order(
            accountId: cmd.AccountId,
            instrumentId: r.InstrumentId,
            side: r.Side,
            type: r.Type,
            quantity: r.Quantity,
            limitPrice: r.LimitPrice
        );

        order.Accept();

        await _orders.AddAsync(order, ct);
        await _jobs.EnqueueAsync(order.Id, ct);

        return new OrderResponse(
            OrderId: order.Id,
            Status: order.Status,
            Quantity: order.Quantity,
            FilledQuantity: order.FilledQuantity,
            RemainingQuantity: order.RemainingQuantity
        );
    }
}
