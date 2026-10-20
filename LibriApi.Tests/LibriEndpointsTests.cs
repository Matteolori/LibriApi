using System.Net;
using System.Net.Http.Json;
using LibriApi.Dtos;
using System.Net.Http.Headers;

namespace LibriApi.Tests;

public class LibriEndpointsTests : IClassFixture<LibriApiFactory>
{
    private readonly HttpClient _client;

    public LibriEndpointsTests(LibriApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AutenticaAsync()
    {
        var credenziali = new { username = $"utente{Guid.NewGuid():N}", password = "Password123!" };

        var registrazione = await _client.PostAsJsonAsync("/api/auth/register", credenziali);
        Assert.Equal(HttpStatusCode.Created, registrazione.StatusCode);

        var login = await _client.PostAsJsonAsync("/api/auth/login", credenziali);
        var token = await login.Content.ReadFromJsonAsync<TokenDto>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token!.Token);
    }

    [Fact]
    public async Task GetTutti_Restituisce200ELibriIniziali()
    {
        var response = await _client.GetAsync("/api/libri");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var libri = await response.Content.ReadFromJsonAsync<List<LibroDto>>();
        Assert.NotNull(libri);
        Assert.True(libri!.Count >= 2);
    }

    [Fact]
    public async Task GetPerId_Inesistente_Restituisce404()
    {
        var response = await _client.GetAsync("/api/libri/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Valido_Restituisce201ConLocation()
    {
        await AutenticaAsync();
        var nuovo = new CreaLibroDto($"Titolo {Guid.NewGuid():N}", "Autore");

        var response = await _client.PostAsJsonAsync("/api/libri", nuovo);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var creato = await response.Content.ReadFromJsonAsync<LibroDto>();
        Assert.NotNull(creato);
        Assert.True(creato!.Id > 0);
        Assert.EndsWith($"/api/libri/{creato.Id}",
            response.Headers.Location!.ToString(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Post_TitoloVuoto_Restituisce400()
    {
        await AutenticaAsync();
        var response = await _client.PostAsJsonAsync("/api/libri", new { titolo = "", autore = "X" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_Duplicato_Restituisce409()
    {
        await AutenticaAsync();
        var libro = new CreaLibroDto($"Titolo {Guid.NewGuid():N}", "Autore");

        var primo = await _client.PostAsJsonAsync("/api/libri", libro);
        var secondo = await _client.PostAsJsonAsync("/api/libri", libro);

        Assert.Equal(HttpStatusCode.Created, primo.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondo.StatusCode);
    }

    [Fact]
    public async Task Delete_LibroEsistente_Restituisce204EPoiGetDa404()
    {
        await AutenticaAsync();
        var nuovo = new CreaLibroDto($"Titolo {Guid.NewGuid():N}", "Autore");
        var creazione = await _client.PostAsJsonAsync("/api/libri", nuovo);
        var creato = await creazione.Content.ReadFromJsonAsync<LibroDto>();

        var eliminazione = await _client.DeleteAsync($"/api/libri/{creato!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, eliminazione.StatusCode);
        
        var verifica = await _client.GetAsync($"/api/libri/{creato.Id}");
        Assert.Equal(HttpStatusCode.NotFound, verifica.StatusCode);
    }

    [Fact]
    public async Task Post_SenzaToken_Restituisce401()
    {
        var nuovo = new CreaLibroDto($"Titolo {Guid.NewGuid():N}", "Autore");

        var response = await _client.PostAsJsonAsync("/api/libri", nuovo);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_PasswordSbagliata_Restituisce401()
    {
        var credenziali = new { username = $"utente{Guid.NewGuid():N}", password = "Password123!" };
        await _client.PostAsJsonAsync("/api/auth/register", credenziali);

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { credenziali.username, password = "SbagliataSbagliata" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}