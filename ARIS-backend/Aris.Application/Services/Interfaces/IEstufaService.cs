using Aris.Application.DTOs.Estufas;

namespace Aris.Application.Services.Interfaces;

public interface IEstufaService
{
    Task<List<EstufaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EstufaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EstufaResponseDto> CreateAsync(CreateEstufaDto request, CancellationToken cancellationToken = default);
    Task<EstufaResponseDto> UpdateAsync(int id, UpdateEstufaDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
