using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Orders;
using MyApp.Infrastructure.Orders;

namespace MyApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
