using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Aris.Application.DTOs.Auth;
using Aris.Application.Interfaces.Repositories;
using Aris.Application.Services.Interfaces;
using Aris.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Aris.Application.Services.Implementations;

public class AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(string email, string senha, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var senhaHash = HashPassword(senha);

        var usuario = await usuarioRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (usuario is null || usuario.SenhaHash != senhaHash)
            throw new InvalidOperationException("Email or password invalid.");

        var expiresMinutesText = configuration["Jwt:ExpiresMinutes"];
        var expiresMinutes = int.TryParse(expiresMinutesText, out var parsedMinutes) ? parsedMinutes : 120;
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);
        var token = BuildToken(usuario, expiresAt);

        return new AuthResponseDto(token, expiresAt, new UsuarioAuthDto(usuario.Id, usuario.Nome, usuario.Email));
    }

    public static string HashPassword(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha.Trim()));
        return Convert.ToHexString(bytes);
    }

    private string BuildToken(Usuario usuario, DateTime expiresAt)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "ARIS";
        var audience = configuration["Jwt:Audience"] ?? "ARIS";
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
