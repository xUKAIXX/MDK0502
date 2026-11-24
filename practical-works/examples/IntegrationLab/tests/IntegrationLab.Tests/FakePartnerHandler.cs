using System.Net;
using System.Net.Http.Json;
using System.Text;
using IntegrationClient;

namespace IntegrationLab.Tests;

/// <summary>
/// Подменяет транспорт: отвечает как настоящий PartnerApi, но без сети.
/// Хранит заказы по ключу идемпотентности, поэтому повтор с тем же ключом
/// возвращает тот же заказ.
/// </summary>
internal sealed class FakePartnerHandler : HttpMessageHandler
{
    private readonly Dictionary<string, OrderResponse> _byKey = new();

    public int Requests { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Requests++;

        if (!request.Headers.Contains("Idempotency-Key"))
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("{\"error\":\"Idempotency-Key is required\"}", Encoding.UTF8, "application/json")
            };
        }

        var key = request.Headers.GetValues("Idempotency-Key").First();

        if (request.Method == HttpMethod.Get)
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{\"error\":\"Order not found\"}", Encoding.UTF8, "application/json")
            };
        }

        var payload = await request.Content!.ReadFromJsonAsync<OrderRequest>(cancellationToken);

        if (payload is null || payload.Amount <= 0)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("{\"error\":\"Amount must be positive\"}", Encoding.UTF8, "application/json")
            };
        }

        if (_byKey.TryGetValue(key, out var existing))
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(existing)
            };
        }

        var order = new OrderResponse(Guid.NewGuid(), "Created", DateTime.UtcNow);
        _byKey[key] = order;

        return new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = JsonContent.Create(order)
        };
    }
}
