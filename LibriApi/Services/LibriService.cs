using LibriApi.Data;
using LibriApi.Dtos;
using LibriApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibriApi.Services;

public class LibriService : ILibriService
{
    private readonly LibriDbContext _db;

    public LibriService(LibriDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<LibroDto>> GetTuttiAsync()
    {
        var libri = await _db.Libri
            .AsNoTracking()
            .OrderBy(l => l.Id)
            .ToListAsync();

        return libri.Select(ToDto).ToList();
    }

    public async Task<LibroDto?> GetPerIdAsync(int id)
    {
        var libro = await _db.Libri.FindAsync(id);
        return libro == null ? null : ToDto(libro);
    }

    public async Task<(EsitoScrittura Esito, LibroDto? Libro)> CreaAsync(CreaLibroDto dto)
    {
        var esiste = await _db.Libri
            .AnyAsync(l => l.Titolo == dto.Titolo && l.Autore == dto.Autore);

        if (esiste)
            return (EsitoScrittura.Duplicato, null);

        var libro = new Libro { Titolo = dto.Titolo, Autore = dto.Autore };
        _db.Libri.Add(libro);
        await _db.SaveChangesAsync();

        return (EsitoScrittura.Ok, ToDto(libro));
    }

    public async Task<EsitoScrittura> AggiornaAsync(int id, AggiornaLibroDto dto)
    {
        var libro = await _db.Libri.FindAsync(id);
        if (libro == null)
            return EsitoScrittura.NonTrovato;

        var duplicato = await _db.Libri
            .AnyAsync(l => l.Id != id && l.Titolo == dto.Titolo && l.Autore == dto.Autore);
        if (duplicato)
            return EsitoScrittura.Duplicato;

        libro.Titolo = dto.Titolo;
        libro.Autore = dto.Autore;
        await _db.SaveChangesAsync();

        return EsitoScrittura.Ok;
    }

    public async Task<bool> EliminaAsync(int id)
    {
        var libro = await _db.Libri.FindAsync(id);
        if (libro == null)
            return false;

        _db.Libri.Remove(libro);
        await _db.SaveChangesAsync();

        return true;
    }

    private static LibroDto ToDto(Libro libro)
    {
        return new LibroDto(libro.Id, libro.Titolo, libro.Autore);
    }
}