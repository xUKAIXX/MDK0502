namespace MyApp.Application.Orders;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    string ItemName,
    decimal Price,
    int Quantity);

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    DateTime CreatedAt,
    string Status,
    decimal Total,
    int ItemsCount);
