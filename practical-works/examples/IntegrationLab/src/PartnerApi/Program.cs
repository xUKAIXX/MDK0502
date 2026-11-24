using System.Collections.Concurrent;
using PartnerApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Счётчик для демонстрации повторов: часть запросов специально завершается 503.
var failCounter = 0;

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "PartnerApi" }));

app.MapGet("/api/orders", () => Results.Ok(OrderStore.All.OrderBy(order => order.CreatedAt)));

app.MapGet("/api/orders/{id:guid}", (Guid id) =>
{
    var order = OrderStore.All.FirstOrDefault(item => item.Id == id);
    return order is null ? Results.NotFound(new { error = "Order not found" }) : Results.Ok(order);
});

app.MapPost("/api/orders", (OrderRequest request, HttpRequest http) =>
{
    var key = http.Headers["Idempotency-Key"].FirstOrDefault();

    if (string.IsNullOrWhiteSpace(key))
    {
        return Results.BadRequest(new { error = "Idempotency-Key is required" });
    }

    if (OrderStore.ByKey.TryGetValue(key, out var existing))
    {
        return Results.Ok(existing);
    }

    if (request.Amount <= 0)
    {
        return Results.BadRequest(new { error = "Amount must be positive" });
    }

    // Демонстрация повтора: если пришёл заголовок X-Demo-Fail, каждый второй
    // запрос завершается 503 — клиент должен повторить его и получить успех.
    if (http.Headers.ContainsKey("X-Demo-Fail"))
    {
        var current = Interlocked.Increment(ref failCounter);

        if (current % 2 == 1)
        {
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    var order = new OrderResponse(Guid.NewGuid(), "Created", DateTime.UtcNow);
    OrderStore.ByKey[key] = order;
    OrderStore.All.Add(order);

    return Results.Created($"/api/orders/{order.Id}", order);
});

app.Run();
