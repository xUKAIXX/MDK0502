using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Dictionaries;

public interface IPropertyTypeRepository
{
    Task<IReadOnlyList<PropertyType>> GetAllAsync(CancellationToken cancellationToken);

    Task<PropertyType?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
