using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Application.Properties;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Web.Controllers.Api;

[ApiController]
[Route("api/properties")]
public sealed class PropertiesApiController(IPropertyService properties) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PropertyListResult>> GetAll(
        [FromQuery] PropertyFilter filter,
        CancellationToken cancellationToken)
    {
        return Ok(await properties.GetPagedAsync(filter, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PropertyDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var property = await properties.GetByIdAsync(id, cancellationToken);

        return property is null ? NotFound() : Ok(property);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<ActionResult<PropertyDto>> Create(
        [FromBody] CreatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerName = User.Identity?.Name;

            var created = await properties.CreateAsync(request, ownerId, ownerName, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DomainException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<ActionResult<PropertyDto>> Update(
        int id,
        [FromBody] UpdatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await properties.UpdateAsync(id, request, cancellationToken);

            return updated is null ? NotFound() : Ok(updated);
        }
        catch (DomainException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await properties.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/status")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> ChangeStatus(
        int id,
        [FromQuery] int status,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(PropertyStatus), status))
        {
            return BadRequest(new { error = "Неизвестный статус объекта." });
        }

        try
        {
            var updated = await properties.ChangeStatusAsync(id, (PropertyStatus)status, cancellationToken);

            return updated ? NoContent() : NotFound();
        }
        catch (DomainException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}
