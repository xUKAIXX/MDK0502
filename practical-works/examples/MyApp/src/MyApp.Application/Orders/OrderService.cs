using Microsoft.Extensions.Options;
using MyApp.Domain.Orders;

namespace MyApp.Application.Orders;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly IClock _clock;
    private readonly OrderOptions _options;

    public OrderService(IOrderRepository orders, IClock clock, IOptions<OrderOptions> options)
    {
        _orders = orders;
        _clock = clock;
        _options = options.Value;
    }

    public async Task<Guid> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.CustomerId, _clock.UtcNow);
        order.AddItem(request.ItemName, request.Price, request.Quantity, _options.MaxItemsPerOrder);

        await _orders.AddAsync(order, cancellationToken);
        return order.Id;
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken);
        return order is null ? null : Map(order);
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var orders = await _orders.GetAllAsync(cancellationToken);
        return orders.Select(Map).ToList();
    }

    public async Task SubmitAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException("Заказ не найден.");

        order.Submit();
    }

    private static OrderDto Map(Order order) => new(
        order.Id,
        order.CustomerId,
        order.CreatedAt,
        order.Status.ToString(),
        order.Total,
        order.Items.Count);
}
