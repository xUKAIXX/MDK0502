using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Properties;

public interface IPropertyService
{
    Task<PropertyListResult> GetPagedAsync(PropertyFilter filter, CancellationToken cancellationToken);

    Task<PropertyDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<PropertyDto> CreateAsync(
        CreatePropertyRequest request,
        string? ownerId,
        string? ownerName,
        CancellationToken cancellationToken);

    Task<PropertyDto?> UpdateAsync(int id, UpdatePropertyRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<bool> ChangeStatusAsync(int id, PropertyStatus status, CancellationToken cancellationToken);
}
