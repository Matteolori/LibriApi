using System.ComponentModel.DataAnnotations;

namespace LibriApi.Dtos;

public record RegistraDto(
    [Required, StringLength(50, MinimumLength = 3)] string Username,
    [Required, StringLength(100, MinimumLength = 8)] string Password);

public record LoginDto(
    [Required] string Username,
    [Required] string Password);

public record TokenDto(string Token);