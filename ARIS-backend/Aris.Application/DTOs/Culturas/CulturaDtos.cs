using System.ComponentModel.DataAnnotations;

namespace Aris.Application.DTOs.Culturas;

public sealed record CreateCulturaDto(
    [param: Required, MinLength(3)] string Nome,
    [param: Range(1, int.MaxValue)] int EstufaId,
    decimal? TempMin,
    decimal? TempMax,
    decimal? UmidadeMin,
    decimal? UmidadeMax);

public sealed record UpdateCulturaDto(
    [param: Required, MinLength(3)] string Nome,
    [param: Range(1, int.MaxValue)] int EstufaId,
    decimal? TempMin,
    decimal? TempMax,
    decimal? UmidadeMin,
    decimal? UmidadeMax);

public sealed record CulturaResponseDto(int Id, string Nome, int EstufaId, int UsuarioId, decimal? TempMin, decimal? TempMax, decimal? UmidadeMin, decimal? UmidadeMax);
