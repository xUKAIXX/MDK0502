using LibraryApp.Application;
using LibraryApp.Application.Contracts;
using LibraryApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Api.Controllers;

[ApiController]
[Route("api/readers")]
public sealed class ReadersController : ControllerBase
{
    private readonly IReaderService _readers;

    public ReadersController(IReaderService readers)
    {
        _readers = readers;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReaderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _readers.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReaderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var reader = await _readers.GetByIdAsync(id, cancellationToken);
        return reader is null ? NotFound() : Ok(reader);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReaderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateReaderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await _readers.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Индивидуальное задание: выдача книги читателю (связь «многие ко многим»).
    [HttpPost("{id:int}/books")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GiveBook(
        int id,
        [FromBody] GiveBookRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _readers.GiveBookAsync(id, request.BookId, cancellationToken);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
