using MyApp.Application.Orders;

namespace MyApp.Infrastructure.Orders;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
