using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Application.Dictionaries;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Repositories;

public sealed class PropertyTypeRepository(ApplicationDbContext context) : IPropertyTypeRepository
{
    public async Task<IReadOnlyList<PropertyType>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.PropertyTypes
            .AsNoTracking()
            .OrderBy(propertyType => propertyType.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PropertyType?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.PropertyTypes
            .FirstOrDefaultAsync(propertyType => propertyType.Id == id, cancellationToken);
    }
}
