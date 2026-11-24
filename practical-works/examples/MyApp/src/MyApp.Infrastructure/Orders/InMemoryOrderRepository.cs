using System.Collections.Concurrent;
using MyApp.Application.Orders;
using MyApp.Domain.Orders;

namespace MyApp.Infrastructure.Orders;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _storage = new();

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _storage.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Order> all = _storage.Values
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        return Task.FromResult(all);
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        _storage[order.Id] = order;
        return Task.CompletedTask;
    }
}
