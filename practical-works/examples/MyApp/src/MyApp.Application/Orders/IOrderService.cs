namespace MyApp.Application.Orders;

public interface IOrderService
{
    Task<Guid> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken);

    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken);

    Task SubmitAsync(Guid id, CancellationToken cancellationToken);
}
