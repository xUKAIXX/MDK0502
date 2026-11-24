using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Application.Dictionaries;

namespace RealEstateAgency.Web.Controllers.Api;

[ApiController]
[Route("api/dictionaries")]
public sealed class DictionariesApiController(IDictionaryService dictionaries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DictionariesDto>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await dictionaries.GetDictionariesAsync(cancellationToken));
    }

    [HttpGet("property-types")]
    public async Task<ActionResult<IReadOnlyList<PropertyTypeDto>>> GetPropertyTypes(CancellationToken cancellationToken)
    {
        var dictionary = await dictionaries.GetDictionariesAsync(cancellationToken);
        return Ok(dictionary.PropertyTypes);
    }

    [HttpGet("districts")]
    public async Task<ActionResult<IReadOnlyList<DistrictDto>>> GetDistricts(CancellationToken cancellationToken)
    {
        var dictionary = await dictionaries.GetDictionariesAsync(cancellationToken);
        return Ok(dictionary.Districts);
    }

    [HttpGet("agents")]
    public async Task<ActionResult<IReadOnlyList<AgentDto>>> GetAgents(CancellationToken cancellationToken)
    {
        var dictionary = await dictionaries.GetDictionariesAsync(cancellationToken);
        return Ok(dictionary.Agents);
    }
}
