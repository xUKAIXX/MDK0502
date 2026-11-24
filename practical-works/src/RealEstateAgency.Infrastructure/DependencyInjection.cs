using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateAgency.Application.Agents;
using RealEstateAgency.Application.Common;
using RealEstateAgency.Application.Deals;
using RealEstateAgency.Application.Dictionaries;
using RealEstateAgency.Application.Properties;
using RealEstateAgency.Infrastructure.Integration;
using RealEstateAgency.Infrastructure.Persistence;
using RealEstateAgency.Infrastructure.Persistence.Repositories;

namespace RealEstateAgency.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=realestate-agency.db";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IPropertyTypeRepository, PropertyTypeRepository>();
        services.AddScoped<IDistrictRepository, DistrictRepository>();
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<IDealRepository, DealRepository>();

        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
