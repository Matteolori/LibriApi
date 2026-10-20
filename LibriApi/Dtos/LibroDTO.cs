using System.ComponentModel.DataAnnotations;

namespace LibriApi.Dtos;

public record LibroDto(int Id, string Titolo, string Autore);

public record CreaLibroDto(
    [Required(ErrorMessage = "Il titolo è obbligatorio.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Il titolo deve avere tra {2} e {1} caratteri.")]
    string Titolo,

    [Required(ErrorMessage = "L'autore è obbligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "L'autore deve avere tra {2} e {1} caratteri.")]
    string Autore);

public record AggiornaLibroDto(
    [Required(ErrorMessage = "Il titolo è obbligatorio.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Il titolo deve avere tra {2} e {1} caratteri.")]
    string Titolo,

    [Required(ErrorMessage = "L'autore è obbligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "L'autore deve avere tra {2} e {1} caratteri.")]
    string Autore);