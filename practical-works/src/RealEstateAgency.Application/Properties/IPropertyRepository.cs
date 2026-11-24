using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Properties;

public interface IPropertyRepository
{
    Task<IReadOnlyList<Property>> GetPagedAsync(PropertyFilter filter, CancellationToken cancellationToken);

    Task<int> CountAsync(PropertyFilter filter, CancellationToken cancellationToken);

    Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(Property property, CancellationToken cancellationToken);

    void Update(Property property);

    void Remove(Property property);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
