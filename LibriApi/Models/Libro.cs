namespace LibriApi.Models;

public class Libro
{
    public int Id { get; set; }
    public string Titolo { get; set; } = string.Empty;
    public string Autore { get; set; } = string.Empty;
    public int? AnnoPubblicazione { get; set; }
}