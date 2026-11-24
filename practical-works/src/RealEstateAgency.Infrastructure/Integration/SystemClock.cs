using RealEstateAgency.Application.Common;

namespace RealEstateAgency.Infrastructure.Integration;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
