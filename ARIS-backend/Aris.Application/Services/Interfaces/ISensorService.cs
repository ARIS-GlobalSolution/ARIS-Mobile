using Aris.Application.DTOs.Sensores;

namespace Aris.Application.Services.Interfaces;

public interface ISensorService
{
    Task<List<SensorResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SensorResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SensorResponseDto> CreateAsync(CreateSensorDto request, CancellationToken cancellationToken = default);
    Task<SensorResponseDto> UpdateAsync(int id, UpdateSensorDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
