using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace IntegrationClient;

public interface IPartnerClient
{
    Task<OrderResponse?> CreateOrderAsync(
        OrderRequest request,
        string idempotencyKey,
        CancellationToken ct);

    Task<OrderResponse?> GetOrderAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<OrderResponse>> GetOrdersAsync(CancellationToken ct);
}

public sealed class PartnerClient(HttpClient http, ILogger<PartnerClient> logger) : IPartnerClient
{
    public async Task<OrderResponse?> CreateOrderAsync(
        OrderRequest request,
        string idempotencyKey,
        CancellationToken ct)
    {
        return await SendAsync<OrderResponse>(
            HttpMethod.Post,
            "/api/orders",
            request,
            idempotencyKey,
            ct);
    }

    public async Task<OrderResponse?> GetOrderAsync(Guid id, CancellationToken ct)
    {
        return await SendAsync<OrderResponse>(HttpMethod.Get, $"/api/orders/{id}", null, null, ct);
    }

    public async Task<IReadOnlyList<OrderResponse>> GetOrdersAsync(CancellationToken ct)
    {
        var orders = await http.GetFromJsonAsync<List<OrderResponse>>("/api/orders", ct);
        return orders ?? new List<OrderResponse>();
    }

    private async Task<T?> SendAsync<T>(
        HttpMethod method,
        string url,
        object? payload,
        string? idempotencyKey,
        CancellationToken ct)
    {
        using var message = new HttpRequestMessage(method, url);

        if (payload is not null)
        {
            message.Content = JsonContent.Create(payload);
        }

        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            message.Headers.Add("Idempotency-Key", idempotencyKey);
        }

        // Корреляционный идентификатор связывает запрос и ответ в логах двух сервисов.
        message.Headers.Add("X-Correlation-Id", Activity.Current?.Id ?? Guid.NewGuid().ToString());

        var stopwatch = Stopwatch.StartNew();

        using var response = await http.SendAsync(message, ct);

        stopwatch.Stop();

        logger.LogInformation(
            "{Method} {Url} -> {StatusCode} за {Elapsed} мс, корреляция {CorrelationId}",
            method.Method,
            url,
            (int)response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            message.Headers.GetValues("X-Correlation-Id").FirstOrDefault());

        // 400 — ошибка клиента: повторять нельзя, нужно исправить запрос.
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            logger.LogWarning("Партнёр отклонил запрос (400): {Error}", error);
            return default;
        }

        // 404 — объект не найден: это не сбой, возвращаем пустой результат.
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning("Партнёр не нашёл объект (404): {Url}", url);
            return default;
        }

        // 429 и 503 попадают сюда только после того, как исчерпаны повторы Polly:
        // здесь превращаем их в управляемый null, чтобы приложение не падало.
        if (response.StatusCode is HttpStatusCode.TooManyRequests
            or HttpStatusCode.ServiceUnavailable)
        {
            logger.LogError(
                "Партнёр недоступен ({StatusCode}) после всех повторов: {Url}",
                (int)response.StatusCode,
                url);
            return default;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(ct);
    }
}
