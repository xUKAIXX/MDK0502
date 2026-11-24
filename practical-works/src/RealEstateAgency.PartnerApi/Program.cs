using RealEstateAgency.PartnerApi;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var byKey = new ConcurrentDictionary<string, MortgageResponse>();
var all = new ConcurrentBag<MortgageResponse>();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "RealEstateAgency.PartnerApi" }));

app.MapPost("/api/mortgage/calculate", (MortgageRequest request, HttpRequest http) =>
{
    var key = http.Headers["Idempotency-Key"].FirstOrDefault();

    if (string.IsNullOrWhiteSpace(key))
    {
        return Results.BadRequest(new { error = "Idempotency-Key is required" });
    }

    if (byKey.TryGetValue(key, out var existing))
    {
        return Results.Ok(existing);
    }

    if (request.Price <= 0 || request.Area <= 0)
    {
        return Results.BadRequest(new { error = "Price and Area must be positive" });
    }

    if (request.TermYears is < 1 or > 30)
    {
        return Results.BadRequest(new { error = "TermYears must be between 1 and 30" });
    }

    if (request.InitialPaymentPercent < 0 || request.InitialPaymentPercent > 90)
    {
        return Results.BadRequest(new { error = "InitialPaymentPercent must be between 0 and 90" });
    }

    var loan = request.Price * (100 - request.InitialPaymentPercent) / 100m;
    var monthlyRate = 0.12m / 12m;
    var months = request.TermYears * 12;
    var factor = (decimal)Math.Pow(1 + (double)monthlyRate, months);
    var monthlyPayment = loan * monthlyRate * factor / (factor - 1);
    monthlyPayment = Math.Round(monthlyPayment, 2);
    var overpayment = Math.Round(monthlyPayment * months - loan, 2);

    var response = new MortgageResponse(
        Guid.NewGuid(),
        request.Address,
        request.Price,
        monthlyPayment,
        overpayment,
        "RUB",
        DateTime.UtcNow);

    byKey[key] = response;
    all.Add(response);

    return Results.Created($"/api/mortgage/calculations/{response.Id}", response);
});

app.MapGet("/api/mortgage/calculations", () => Results.Ok(all.OrderBy(item => item.CreatedAt)));

app.MapGet("/api/mortgage/calculations/{id:guid}", (Guid id) =>
{
    var item = all.FirstOrDefault(value => value.Id == id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.Run();
