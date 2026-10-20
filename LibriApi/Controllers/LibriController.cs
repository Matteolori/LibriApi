using LibriApi.Dtos;
using LibriApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibriApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibriController : ControllerBase
{
    private readonly ILibriService _service;

    public LibriController(ILibriService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetTutti()
    {
        return Ok(await _service.GetTuttiAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LibroDto>> GetPerId(int id)
    {
        var libro = await _service.GetPerIdAsync(id);

        if (libro == null)
            return NotFound();

        return Ok(libro);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LibroDto>> Crea(CreaLibroDto dto)
    {
        var (esito, libro) = await _service.CreaAsync(dto);

        if (esito == EsitoScrittura.Duplicato)
            return Problem(
                detail: "Esiste già un libro con questo titolo e autore.",
                statusCode: StatusCodes.Status409Conflict);

        return CreatedAtAction(nameof(GetPerId), new { id = libro!.Id }, libro);
    }
    
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Aggiorna(int id, AggiornaLibroDto dto)
    {
        var esito = await _service.AggiornaAsync(id, dto);

        return esito switch
        {
            EsitoScrittura.NonTrovato => NotFound(),
            EsitoScrittura.Duplicato => Problem(
                detail: "Esiste già un libro con questo titolo e autore.",
                statusCode: StatusCodes.Status409Conflict),
            _ => NoContent()
        };
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Elimina(int id)
    {
        var eliminato = await _service.EliminaAsync(id);

        if (!eliminato)
            return NotFound();

        return NoContent();
    }

    
}