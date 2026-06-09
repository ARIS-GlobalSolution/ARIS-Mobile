using System.ComponentModel.DataAnnotations;

namespace Aris.Application.DTOs.Estufas;

public sealed record CreateEstufaDto(
    [param: Required, MinLength(3)] string Nome,
    string? Localizacao,
    [param: Range(1, int.MaxValue)] int UsuarioId);

public sealed record UpdateEstufaDto(
    [param: Required, MinLength(3)] string Nome,
    string? Localizacao,
    [param: Range(1, int.MaxValue)] int UsuarioId);

public sealed record EstufaResponseDto(int Id, string Nome, string? Localizacao, int UsuarioId);
