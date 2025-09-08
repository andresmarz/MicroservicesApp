using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Contracts;

    public record OrderSubmitted(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);

