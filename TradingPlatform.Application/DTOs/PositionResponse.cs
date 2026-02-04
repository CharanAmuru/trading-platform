using System;
using System.Collections.Generic;
using System.Text;

namespace TradingPlatform.Application.DTOs;

public sealed record PositionResponse(
    Guid AccountId,
    Guid InstrumentId,
    decimal Quantity,
    decimal AvgPrice,
    DateTime UpdatedAtUtc
);
