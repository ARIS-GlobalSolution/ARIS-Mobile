namespace Aris.Application.DTOs.Auth;

public sealed record AuthResponseDto(string AccessToken, DateTime ExpiresAt, UsuarioAuthDto Usuario);

public sealed record UsuarioAuthDto(int Id, string Nome, string Email);
