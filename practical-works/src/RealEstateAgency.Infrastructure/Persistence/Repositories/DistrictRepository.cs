using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Application.Dictionaries;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Repositories;

public sealed class DistrictRepository(ApplicationDbContext context) : IDistrictRepository
{
    public async Task<IReadOnlyList<District>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Districts
            .AsNoTracking()
            .OrderBy(district => district.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<District?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Districts
            .FirstOrDefaultAsync(district => district.Id == id, cancellationToken);
    }
}
