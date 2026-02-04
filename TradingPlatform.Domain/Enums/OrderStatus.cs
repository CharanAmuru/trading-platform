using System;
using System.Collections.Generic;
using System.Text;

namespace TradingPlatform.Domain.Enums
{
    public enum OrderStatus
    {
        New = 0,
        Accepted = 1,
        PartiallyFilled = 2,
        Filled = 3,
        Canceled = 4,
        Rejected = 5
    }

}
