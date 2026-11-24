namespace IntegrationClient;

public sealed record OrderRequest(string CustomerId, decimal Amount, string Currency);

public sealed record OrderResponse(Guid Id, string Status, DateTime CreatedAt);
