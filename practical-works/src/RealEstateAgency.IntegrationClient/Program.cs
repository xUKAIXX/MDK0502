using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using RealEstateAgency.IntegrationClient;

var services = new ServiceCollection();

services.AddLogging(builder => builder.AddConsole());

services.AddHttpClient<IMortgageApiClient, MortgageApiClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5100");
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IMortgageApiClient>();

var request = new MortgageRequest(
    "Невский проспект, 100",
    12_500_000m,
    65.4,
    2,
    20,
    20m);

var key = Guid.NewGuid().ToString("N");

Console.WriteLine("=== Успешный расчёт ипотеки ===");
var first = await client.CalculateAsync(request, key, CancellationToken.None);
Console.WriteLine($"Первый запрос: {first?.Id}, платёж {first?.MonthlyPayment} ₽/мес");

Console.WriteLine("\n=== Повторный запрос с тем же Idempotency-Key ===");
var second = await client.CalculateAsync(request, key, CancellationToken.None);
Console.WriteLine($"Повторный запрос: {second?.Id}, платёж {second?.MonthlyPayment} ₽/мес");
Console.WriteLine($"Тот же результат: {first?.Id == second?.Id}");

Console.WriteLine("\n=== Некорректный запрос (сумма 0) ===");
var invalid = await client.CalculateAsync(
    new MortgageRequest("Невский проспект, 100", 0m, 65.4, 2),
    Guid.NewGuid().ToString("N"),
    CancellationToken.None);
Console.WriteLine(invalid is null
    ? "Ошибка 400 обработана без исключения, клиент вернул null."
    : "Ошибка не обработана.");

Console.WriteLine("\n=== Сохранённые расчёты ===");
var all = await client.GetAllAsync(CancellationToken.None);
Console.WriteLine($"Всего расчётов: {all.Count}");

Console.WriteLine("\nНажмите Enter для выхода...");
Console.ReadLine();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(message => message.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)));

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
