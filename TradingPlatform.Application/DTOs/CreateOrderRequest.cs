using System.Text.Json.Serialization;
using TradingPlatform.Domain.Enums;

namespace TradingPlatform.Application.DTOs;

public sealed class CreateOrderRequest
{
    [JsonPropertyName("instrumentId")]
    public Guid InstrumentId { get; set; }

    [JsonPropertyName("side")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderSide Side { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderType Type { get; set; }

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("limitPrice")]
    public decimal? LimitPrice { get; set; }
}
