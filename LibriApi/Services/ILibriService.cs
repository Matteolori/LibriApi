using LibriApi.Dtos;

namespace LibriApi.Services;

public enum EsitoScrittura
{
    Ok,
    NonTrovato,
    Duplicato
}

public interface ILibriService
{
    Task<IReadOnlyList<LibroDto>> GetTuttiAsync();
    Task<LibroDto?> GetPerIdAsync(int id);
    Task<(EsitoScrittura Esito, LibroDto? Libro)> CreaAsync(CreaLibroDto dto);
    Task<EsitoScrittura> AggiornaAsync(int id, AggiornaLibroDto dto);
    Task<bool> EliminaAsync(int id);
}