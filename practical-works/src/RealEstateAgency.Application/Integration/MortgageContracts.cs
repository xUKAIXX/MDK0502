namespace RealEstateAgency.Application.Integration;

public sealed record MortgageRequest(
    string Address,
    decimal Price,
    double Area,
    int Rooms,
    int TermYears = 20,
    decimal InitialPaymentPercent = 20m);

public sealed record MortgageResponse(
    Guid Id,
    string Address,
    decimal EstimatedPrice,
    decimal MonthlyPayment,
    decimal Overpayment,
    string Currency,
    DateTime CreatedAt);
