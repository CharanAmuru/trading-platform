using System.Text.Json.Serialization;

namespace TradingPlatform.Application.DTOs;

public sealed class PlaceOrderRequest
{
    [JsonPropertyName("request")]
    public CreateOrderRequest Request { get; set; } = new();
}
