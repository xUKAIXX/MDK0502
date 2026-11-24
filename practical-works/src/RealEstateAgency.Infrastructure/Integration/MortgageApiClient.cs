using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using RealEstateAgency.Application.Integration;

namespace RealEstateAgency.Infrastructure.Integration;

public sealed class MortgageApiClient(
    HttpClient http,
    ILogger<MortgageApiClient> logger) : IMortgageApiClient
{
    public async Task<MortgageResponse?> CalculateAsync(
        MortgageRequest request,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "/api/mortgage/calculate")
        {
            Content = JsonContent.Create(request)
        };

        message.Headers.Add("Idempotency-Key", idempotencyKey);
        message.Headers.Add("X-Correlation-Id", Activity.Current?.Id ?? Guid.NewGuid().ToString());

        using var response = await http.SendAsync(message, cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("Партнёрский сервис отклонил запрос: {Error}", error);
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MortgageResponse>(cancellationToken);
    }

    public async Task<IReadOnlyList<MortgageResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var values = await http.GetFromJsonAsync<List<MortgageResponse>>(
            "/api/mortgage/calculations",
            cancellationToken);

        return values ?? new List<MortgageResponse>();
    }
}
