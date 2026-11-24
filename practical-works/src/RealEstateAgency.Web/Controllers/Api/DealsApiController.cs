using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Application.Deals;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Web.Controllers.Api;

[ApiController]
[Route("api/deals")]
public sealed class DealsApiController(IDealService deals) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DealDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await deals.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DealDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var deal = await deals.GetByIdAsync(id, cancellationToken);

        return deal is null ? NotFound() : Ok(deal);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<DealDto>> Create(
        [FromBody] CreateDealRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await deals.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DomainException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("{id:int}/start")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Start(int id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await deals.StartAsync(id, cancellationToken);
            return updated ? NoContent() : NotFound();
        }
        catch (DomainException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("{id:int}/complete")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Complete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await deals.CompleteAsync(id, cancellationToken);
            return updated ? NoContent() : NotFound();
        }
        catch (DomainException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await deals.CancelAsync(id, cancellationToken);
            return updated ? NoContent() : NotFound();
        }
        catch (DomainException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}
