namespace MyApp.Domain.Orders;

public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
