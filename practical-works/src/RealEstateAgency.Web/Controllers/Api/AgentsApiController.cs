using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.Application.Agents;
using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Web.Controllers.Api;

[ApiController]
[Route("api/agents")]
public sealed class AgentsApiController(IAgentService agents) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AgentDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await agents.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AgentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var agent = await agents.GetByIdAsync(id, cancellationToken);

        return agent is null ? NotFound() : Ok(agent);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AgentDto>> Create(
        [FromBody] CreateAgentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await agents.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DomainException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AgentDto>> Update(
        int id,
        [FromBody] UpdateAgentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await agents.UpdateAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (DomainException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await agents.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
