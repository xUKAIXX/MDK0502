using RealEstateAgency.Application.Agents;
using RealEstateAgency.Application.Contracts;

namespace RealEstateAgency.Application.Dictionaries;

public sealed class DictionaryService : IDictionaryService
{
    private readonly IPropertyTypeRepository _propertyTypes;
    private readonly IDistrictRepository _districts;
    private readonly IAgentRepository _agents;

    public DictionaryService(
        IPropertyTypeRepository propertyTypes,
        IDistrictRepository districts,
        IAgentRepository agents)
    {
        _propertyTypes = propertyTypes;
        _districts = districts;
        _agents = agents;
    }

    public async Task<DictionariesDto> GetDictionariesAsync(CancellationToken cancellationToken)
    {
        var propertyTypes = await _propertyTypes.GetAllAsync(cancellationToken);
        var districts = await _districts.GetAllAsync(cancellationToken);
        var agents = await _agents.GetAllAsync(cancellationToken);

        return new DictionariesDto(
            propertyTypes.Select(x => new PropertyTypeDto(x.Id, x.Name)).OrderBy(x => x.Name).ToList(),
            districts.Select(x => new DistrictDto(x.Id, x.Name, x.City)).OrderBy(x => x.Name).ToList(),
            agents.Select(x => new AgentDto(x.Id, x.FullName, x.Phone, x.Email, x.ExperienceYears, x.Properties.Count))
                .OrderBy(x => x.FullName)
                .ToList());
    }
}
