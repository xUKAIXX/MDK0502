namespace MyApp.Application.Orders;

public interface IClock
{
    DateTime UtcNow { get; }
}
