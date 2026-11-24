using System.Net;
using System.Net.Http.Json;
using IntegrationClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

var services = new ServiceCollection();

services.AddLogging(builder => builder.AddConsole());

services.AddHttpClient<IPartnerClient, PartnerClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5100");
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// Отдельный именованный клиент для демонстрации повтора при 503.
services.AddHttpClient("demo-retry", client =>
    {
        client.BaseAddress = new Uri("http://localhost:5100");
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IPartnerClient>();

// 1. Успешное создание заказа.
var request = new OrderRequest("customer-1", 1500m, "RUB");
var key = Guid.NewGuid().ToString("N");

Console.WriteLine("=== 1. Создание заказа ===");
var first = await client.CreateOrderAsync(request, key, CancellationToken.None);
Console.WriteLine($"First:  {first?.Id}, {first?.Status}");

// 2. Повтор с тем же ключом идемпотентности.
Console.WriteLine("\n=== 2. Повторный запрос с тем же Idempotency-Key ===");
var second = await client.CreateOrderAsync(request, key, CancellationToken.None);
Console.WriteLine($"Second: {second?.Id}, {second?.Status}");
Console.WriteLine($"Same id: {first?.Id == second?.Id}");

// 3. Чтение списка заказов.
Console.WriteLine("\n=== 3. Список заказов ===");
var all = await client.GetOrdersAsync(CancellationToken.None);
Console.WriteLine($"Всего заказов: {all.Count}");

// 4. Ошибка 400: сумма 0 — клиент возвращает null без исключения.
Console.WriteLine("\n=== 4. Некорректный запрос (Amount = 0) ===");
var invalid = await client.CreateOrderAsync(
    new OrderRequest("customer-1", 0m, "RUB"),
    Guid.NewGuid().ToString("N"),
    CancellationToken.None);
Console.WriteLine(invalid is null
    ? "Ошибка 400 обработана: клиент вернул null, исключения нет."
    : "Ошибка не обработана.");

// 5. Ошибка 404: заказ с несуществующим идентификатором.
Console.WriteLine("\n=== 5. Чтение несуществующего заказа (404) ===");
var missing = await client.GetOrderAsync(Guid.NewGuid(), CancellationToken.None);
Console.WriteLine(missing is null
    ? "Ошибка 404 обработана: клиент вернул null."
    : "Ошибка не обработана.");

// 6. Временная ошибка 503: партнёр отвечает 503 через раз,
//    поэтому первый запрос завершается ошибкой, а повтор — успехом.
Console.WriteLine("\n=== 6. Временная ошибка 503 и автоматический повтор ===");
var factory = provider.GetRequiredService<IHttpClientFactory>();

using (var demoClient = factory.CreateClient("demo-retry"))
{
    demoClient.DefaultRequestHeaders.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
    demoClient.DefaultRequestHeaders.Add("X-Demo-Fail", "true");

    using var response = await demoClient.PostAsJsonAsync("/api/orders", request);
    Console.WriteLine($"Итоговый код ответа после повторов: {(int)response.StatusCode}");
}

// 7. Запрос без Idempotency-Key — партнёр вернёт 400.
Console.WriteLine("\n=== 7. Запрос без Idempotency-Key ===");
var noKey = await client.CreateOrderAsync(request, string.Empty, CancellationToken.None);
Console.WriteLine(noKey is null
    ? "Ошибка 400 обработана: без ключа заказ не создаётся."
    : "Ошибка не обработана.");

Console.WriteLine("\nНажмите Enter для выхода...");
Console.ReadLine();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, attempt =>
            TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)));

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
