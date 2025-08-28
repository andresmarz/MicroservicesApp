namespace EventBus.Contracts;

public record OrderSubmitted(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);

