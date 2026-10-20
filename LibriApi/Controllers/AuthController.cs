using LibriApi.Data;
using LibriApi.Dtos;
using LibriApi.Models;
using LibriApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibriApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LibriDbContext _db;
    private readonly IPasswordHasher<Utente> _hasher;
    private readonly ITokenService _tokens;

    public AuthController(LibriDbContext db, IPasswordHasher<Utente> hasher, ITokenService tokens)
    {
        _db = db;
        _hasher = hasher;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Registra(RegistraDto dto)
    {
        if (await _db.Utenti.AnyAsync(u => u.Username == dto.Username))
            return Problem(
                detail: "Username già in uso.",
                statusCode: StatusCodes.Status409Conflict);

        var utente = new Utente { Username = dto.Username };
        utente.PasswordHash = _hasher.HashPassword(utente, dto.Password);

        _db.Utenti.Add(utente);
        await _db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
    {
        var utente = await _db.Utenti.FirstOrDefaultAsync(u => u.Username == dto.Username);

        var valido = utente != null &&
            _hasher.VerifyHashedPassword(utente, utente.PasswordHash, dto.Password)
                != PasswordVerificationResult.Failed;

        if (!valido)
            return Problem(
                detail: "Credenziali non valide.",
                statusCode: StatusCodes.Status401Unauthorized);

        return Ok(new TokenDto(_tokens.CreaToken(utente!)));
    }
}