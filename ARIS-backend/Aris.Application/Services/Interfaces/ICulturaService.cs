using Aris.Application.DTOs.Culturas;

namespace Aris.Application.Services.Interfaces;

public interface ICulturaService
{
    Task<List<CulturaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CulturaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CulturaResponseDto> CreateAsync(CreateCulturaDto request, CancellationToken cancellationToken = default);
    Task<CulturaResponseDto> UpdateAsync(int id, UpdateCulturaDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
