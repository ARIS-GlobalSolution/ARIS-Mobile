using Aris.Application.DTOs.Auth;

namespace Aris.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(string email, string senha, CancellationToken cancellationToken = default);
}
