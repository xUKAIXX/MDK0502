using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Application.Properties;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Repositories;

public sealed class PropertyRepository(ApplicationDbContext context) : IPropertyRepository
{
    public async Task<IReadOnlyList<Property>> GetPagedAsync(PropertyFilter filter, CancellationToken cancellationToken)
    {
        var query = ApplySorting(ApplyFilters(context.Properties.AsNoTracking(), filter), filter);

        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        return await query
            .Include(property => property.PropertyType)
            .Include(property => property.District)
            .Include(property => property.Agent)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(PropertyFilter filter, CancellationToken cancellationToken)
    {
        return await ApplyFilters(context.Properties.AsNoTracking(), filter)
            .CountAsync(cancellationToken);
    }

    public async Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Properties
            .Include(property => property.PropertyType)
            .Include(property => property.District)
            .Include(property => property.Agent)
            .FirstOrDefaultAsync(property => property.Id == id, cancellationToken);
    }

    public async Task AddAsync(Property property, CancellationToken cancellationToken)
    {
        await context.Properties.AddAsync(property, cancellationToken);
    }

    public void Update(Property property)
    {
        context.Properties.Update(property);
    }

    public void Remove(Property property)
    {
        context.Properties.Remove(property);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Property> ApplyFilters(IQueryable<Property> query, PropertyFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            query = query.Where(property =>
                property.Title.ToLower().Contains(term) ||
                property.Address.ToLower().Contains(term) ||
                property.Agent.FullName.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(filter.Status) && int.TryParse(filter.Status, out var status))
        {
            query = query.Where(property => (int)property.Status == status);
        }

        if (filter.PropertyTypeId.HasValue)
        {
            query = query.Where(property => property.PropertyTypeId == filter.PropertyTypeId.Value);
        }

        if (filter.DistrictId.HasValue)
        {
            query = query.Where(property => property.DistrictId == filter.DistrictId.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(property => property.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(property => property.Price <= filter.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.OwnerId))
        {
            query = query.Where(property => property.OwnerId == filter.OwnerId);
        }

        return query;
    }

    private static IQueryable<Property> ApplySorting(IQueryable<Property> query, PropertyFilter filter)
    {
        var descending = string.Equals(filter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return filter.SortBy?.ToLowerInvariant() switch
        {
            "price" => descending
                ? query.OrderByDescending(property => property.Price)
                : query.OrderBy(property => property.Price),
            "area" => descending
                ? query.OrderByDescending(property => property.Area)
                : query.OrderBy(property => property.Area),
            "year" => descending
                ? query.OrderByDescending(property => property.YearBuilt)
                : query.OrderBy(property => property.YearBuilt),
            "title" => descending
                ? query.OrderByDescending(property => property.Title)
                : query.OrderBy(property => property.Title),
            _ => query.OrderByDescending(property => property.CreatedAt)
        };
    }
}
