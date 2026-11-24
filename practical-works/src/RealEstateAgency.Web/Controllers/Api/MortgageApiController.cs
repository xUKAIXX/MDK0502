using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.Application.Integration;

namespace RealEstateAgency.Web.Controllers.Api;

[ApiController]
[Route("api/mortgage")]
public sealed class MortgageApiController(IMortgageApiClient mortgageApi) : ControllerBase
{
    [HttpPost("calculate")]
    [Authorize]
    public async Task<IActionResult> Calculate(
        [FromBody] MortgageRequest request,
        CancellationToken cancellationToken)
    {
        var key = Request.Headers["Idempotency-Key"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        var result = await mortgageApi.CalculateAsync(request, key, cancellationToken);

        return result is null
            ? BadRequest(new { error = "Партнёрский сервис отклонил запрос." })
            : Ok(result);
    }

    [HttpGet("calculations")]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await mortgageApi.GetAllAsync(cancellationToken));
    }
}
