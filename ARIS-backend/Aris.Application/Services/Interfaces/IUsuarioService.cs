using Aris.Application.DTOs.Usuarios;

namespace Aris.Application.Services.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UsuarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UsuarioResponseDto> CreateAsync(CreateUsuarioDto request, CancellationToken cancellationToken = default);
    Task<UsuarioResponseDto> UpdateAsync(int id, UpdateUsuarioDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
