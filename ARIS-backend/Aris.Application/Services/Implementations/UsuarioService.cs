using Aris.Application.DTOs.Usuarios;
using Aris.Application.Helpers;
using Aris.Application.Interfaces.Repositories;
using Aris.Application.Services.Interfaces;
using Aris.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aris.Application.Services.Implementations;

public class UsuarioService(IUsuarioRepository usuarioRepository) : IUsuarioService
{
    public async Task<List<UsuarioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var usuarios = await usuarioRepository.GetAllAsync(cancellationToken);
        return usuarios.Select(Map).ToList();
    }

    public async Task<UsuarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.GetByIdAsync(id, cancellationToken);
        return usuario is null ? null : Map(usuario);
    }

    public async Task<UsuarioResponseDto> CreateAsync(CreateUsuarioDto request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await usuarioRepository.GetByEmailAsync(normalizedEmail, cancellationToken) is not null)
            throw new InvalidOperationException("Email already exists.");

        var usuario = new Usuario(request.Nome, normalizedEmail, AuthService.HashPassword(request.Senha));
        EntityIdHelper.SetId(usuario, await EntityIdHelper.GetNextIdAsync(usuarioRepository, cancellationToken));
        await usuarioRepository.AddAsync(usuario, cancellationToken);
        await usuarioRepository.SaveChangesAsync(cancellationToken);
        return Map(usuario);
    }

    public async Task<UsuarioResponseDto> UpdateAsync(int id, UpdateUsuarioDto request, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario not found.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var emailExists = await usuarioRepository.Query()
            .CountAsync(x => x.Email == normalizedEmail && x.Id != id, cancellationToken) > 0;
        if (emailExists)
            throw new InvalidOperationException("Email already exists.");

        usuario.Update(request.Nome, normalizedEmail, AuthService.HashPassword(request.Senha));
        usuarioRepository.Update(usuario);
        await usuarioRepository.SaveChangesAsync(cancellationToken);
        return Map(usuario);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario not found.");

        usuarioRepository.Remove(usuario);
        await usuarioRepository.SaveChangesAsync(cancellationToken);
    }

    private static UsuarioResponseDto Map(Usuario usuario) => new(usuario.Id, usuario.Nome, usuario.Email, usuario.DataCadastro);
}
