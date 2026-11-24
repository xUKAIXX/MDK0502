using System.Collections.Concurrent;

namespace PartnerApi;

public sealed record OrderRequest(string CustomerId, decimal Amount, string Currency);

public sealed record OrderResponse(Guid Id, string Status, DateTime CreatedAt);

/// <summary>
/// Хранилище заказов в памяти. Ключ — Idempotency-Key: повторный запрос
/// с тем же ключом возвращает уже созданный заказ, а не создаёт дубль.
/// </summary>
public static class OrderStore
{
    public static readonly ConcurrentDictionary<string, OrderResponse> ByKey = new();

    public static readonly ConcurrentBag<OrderResponse> All = new();
}
