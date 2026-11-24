using RealEstateAgency.Application.Agents;
using RealEstateAgency.Application.Common;
using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Application.Dictionaries;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Properties;

public sealed class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _properties;
    private readonly IPropertyTypeRepository _propertyTypes;
    private readonly IDistrictRepository _districts;
    private readonly IAgentRepository _agents;
    private readonly IClock _clock;

    public PropertyService(
        IPropertyRepository properties,
        IPropertyTypeRepository propertyTypes,
        IDistrictRepository districts,
        IAgentRepository agents,
        IClock clock)
    {
        _properties = properties;
        _propertyTypes = propertyTypes;
        _districts = districts;
        _agents = agents;
        _clock = clock;
    }

    public async Task<PropertyListResult> GetPagedAsync(PropertyFilter filter, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);
        var normalized = filter with { Page = page, PageSize = pageSize };

        var total = await _properties.CountAsync(normalized, cancellationToken);
        var items = await _properties.GetPagedAsync(normalized, cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new PropertyListResult(
            items.Select(MapToListItem).ToList(),
            total,
            page,
            pageSize,
            totalPages);
    }

    public async Task<PropertyDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var property = await _properties.GetByIdAsync(id, cancellationToken);
        return property is null ? null : Map(property);
    }

    public async Task<PropertyDto> CreateAsync(
        CreatePropertyRequest request,
        string? ownerId,
        string? ownerName,
        CancellationToken cancellationToken)
    {
        var propertyType = await _propertyTypes.GetByIdAsync(request.PropertyTypeId, cancellationToken)
            ?? throw new DomainException("Тип объекта не найден.");

        var district = await _districts.GetByIdAsync(request.DistrictId, cancellationToken)
            ?? throw new DomainException("Район не найден.");

        var agent = await _agents.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new DomainException("Агент не найден.");

        var property = Property.Create(
            request.Title,
            request.Address,
            request.Price,
            request.Area,
            request.Rooms,
            request.Floor,
            request.TotalFloors,
            request.YearBuilt,
            request.Description,
            propertyType,
            district,
            agent,
            ownerId,
            ownerName,
            _clock.UtcNow);

        await _properties.AddAsync(property, cancellationToken);
        await _properties.SaveChangesAsync(cancellationToken);

        return Map(property);
    }

    public async Task<PropertyDto?> UpdateAsync(int id, UpdatePropertyRequest request, CancellationToken cancellationToken)
    {
        var property = await _properties.GetByIdAsync(id, cancellationToken);
        if (property is null)
        {
            return null;
        }

        var propertyType = await _propertyTypes.GetByIdAsync(request.PropertyTypeId, cancellationToken)
            ?? throw new DomainException("Тип объекта не найден.");

        var district = await _districts.GetByIdAsync(request.DistrictId, cancellationToken)
            ?? throw new DomainException("Район не найден.");

        var agent = await _agents.GetByIdAsync(request.AgentId, cancellationToken)
            ?? throw new DomainException("Агент не найден.");

        property.UpdateDetails(
            request.Title,
            request.Address,
            request.Price,
            request.Area,
            request.Rooms,
            request.Floor,
            request.TotalFloors,
            request.YearBuilt,
            request.Description,
            propertyType,
            district,
            agent);

        _properties.Update(property);
        await _properties.SaveChangesAsync(cancellationToken);

        return Map(property);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var property = await _properties.GetByIdAsync(id, cancellationToken);
        if (property is null)
        {
            return false;
        }

        _properties.Remove(property);
        await _properties.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ChangeStatusAsync(int id, PropertyStatus status, CancellationToken cancellationToken)
    {
        var property = await _properties.GetByIdAsync(id, cancellationToken);
        if (property is null)
        {
            return false;
        }

        property.ChangeStatus(status);
        await _properties.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PropertyDto Map(Property property) => new(
        property.Id,
        property.Title,
        property.Address,
        property.Price,
        property.Area,
        property.Rooms,
        property.Floor,
        property.TotalFloors,
        property.YearBuilt,
        property.Description,
        property.Status.ToRussian(),
        property.PropertyTypeId,
        property.PropertyType.Name,
        property.DistrictId,
        property.District.Name,
        property.AgentId,
        property.Agent.FullName,
        property.OwnerName,
        property.CreatedAt);

    private static PropertyListItemDto MapToListItem(Property property) => new(
        property.Id,
        property.Title,
        property.Address,
        property.Price,
        property.Area,
        property.Rooms,
        property.Status.ToRussian(),
        property.PropertyType.Name,
        property.District.Name);
}
