using System.ComponentModel.DataAnnotations;

namespace Aris.Application.DTOs.Sensores;

public sealed record CreateSensorDto(
    [param: Range(1, int.MaxValue)] int TipoSensorId,
    [param: Range(1, int.MaxValue)] int EstufaId);

public sealed record UpdateSensorDto(
    [param: Range(1, int.MaxValue)] int TipoSensorId,
    [param: Range(1, int.MaxValue)] int EstufaId);

public sealed record SensorResponseDto(int Id, int TipoSensorId, int EstufaId);
