using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Dictionaries;

public interface IDistrictRepository
{
    Task<IReadOnlyList<District>> GetAllAsync(CancellationToken cancellationToken);

    Task<District?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
