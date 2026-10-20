using LibriApi.Data;
using LibriApi.Dtos;
using LibriApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LibriApi.Tests;

public class LibriServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly LibriDbContext _db;
    private readonly LibriService _service;

    public LibriServiceTests()
    {
        // SQLite in memoria: vive finché la connessione resta aperta
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<LibriDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new LibriDbContext(options);
        _db.Database.EnsureCreated();

        _service = new LibriService(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task Crea_LibroNuovo_RestituisceOk()
    {
        var (esito, libro) = await _service.CreaAsync(new CreaLibroDto("Test", "Autore"));

        Assert.Equal(EsitoScrittura.Ok, esito);
        Assert.NotNull(libro);
        Assert.Equal("Test", libro!.Titolo);
    }

    [Fact]
    public async Task Crea_Duplicato_RestituisceDuplicato()
    {
        await _service.CreaAsync(new CreaLibroDto("Test", "Autore"));

        var (esito, libro) = await _service.CreaAsync(new CreaLibroDto("Test", "Autore"));

        Assert.Equal(EsitoScrittura.Duplicato, esito);
        Assert.Null(libro);
    }

    [Fact]
    public async Task Elimina_IdInesistente_RestituisceFalse()
    {
        var eliminato = await _service.EliminaAsync(9999);

        Assert.False(eliminato);
    }

    [Fact]
    public async Task Aggiorna_IdInesistente_RestituisceNonTrovato()
    {
        var dto = new AggiornaLibroDto("Titolo", "Autore");

        var esito = await _service.AggiornaAsync(9999, dto);

        Assert.Equal(EsitoScrittura.NonTrovato, esito);
    }

    [Fact]
    public async Task Aggiorna_VersoDuplicato_RestituisceDuplicato()
    {
        await _service.CreaAsync(new CreaLibroDto("Titolo1", "Autore1"));
        var (_, secondo) = await _service.CreaAsync(new CreaLibroDto("Titolo2", "Autore2"));

        var esito = await _service.AggiornaAsync(
            secondo!.Id,
            new AggiornaLibroDto("Titolo1", "Autore1"));

        Assert.Equal(EsitoScrittura.Duplicato, esito);
    }
}