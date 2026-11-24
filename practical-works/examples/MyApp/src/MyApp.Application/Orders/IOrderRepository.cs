using MyApp.Domain.Orders;

namespace MyApp.Application.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Order order, CancellationToken cancellationToken);
}
