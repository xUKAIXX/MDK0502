namespace RealEstateAgency.Application.Integration;

public interface IMortgageApiClient
{
    Task<MortgageResponse?> CalculateAsync(MortgageRequest request, string idempotencyKey, CancellationToken cancellationToken);

    Task<IReadOnlyList<MortgageResponse>> GetAllAsync(CancellationToken cancellationToken);
}
