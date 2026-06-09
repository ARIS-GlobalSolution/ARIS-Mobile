using System.Security.Claims;
using Aris.Application.DTOs.Estufas;
using Aris.Application.Helpers;
using Aris.Application.Interfaces.Repositories;
using Aris.Application.Services.Interfaces;
using Aris.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Aris.Application.Services.Implementations;

public class EstufaService(
    IEstufaRepository estufaRepository,
    IUsuarioRepository usuarioRepository,
    IHttpContextAccessor httpContextAccessor) : IEstufaService
{
    public async Task<List<EstufaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var query = estufaRepository.Query();

        if (currentUserId is not null)
        {
            query = query.Where(x => x.UsuarioId == currentUserId.Value);
        }

        var estufas = await query.ToListAsync(cancellationToken);
        return estufas.Select(Map).ToList();
    }

    public async Task<EstufaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var estufa = await estufaRepository.GetByIdAsync(id, cancellationToken);
        if (estufa is null || !BelongsToCurrentUser(estufa))
            return null;

        return Map(estufa);
    }

    public async Task<EstufaResponseDto> CreateAsync(CreateEstufaDto request, CancellationToken cancellationToken = default)
    {
        var currentUserId = await EnsureCurrentUserAsync(cancellationToken);
        var estufa = new Estufa(request.Nome, request.Localizacao, currentUserId);
        EntityIdHelper.SetId(estufa, await EntityIdHelper.GetNextIdAsync(estufaRepository, cancellationToken));
        await estufaRepository.AddAsync(estufa, cancellationToken);
        await estufaRepository.SaveChangesAsync(cancellationToken);
        return Map(estufa);
    }

    public async Task<EstufaResponseDto> UpdateAsync(int id, UpdateEstufaDto request, CancellationToken cancellationToken = default)
    {
        var estufa = await estufaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Estufa not found.");

        if (!BelongsToCurrentUser(estufa))
            throw new UnauthorizedAccessException("Estufa not found.");

        var currentUserId = await EnsureCurrentUserAsync(cancellationToken);
        estufa.Update(request.Nome, request.Localizacao, currentUserId);
        estufaRepository.Update(estufa);
        await estufaRepository.SaveChangesAsync(cancellationToken);
        return Map(estufa);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var estufa = await estufaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Estufa not found.");

        if (!BelongsToCurrentUser(estufa))
            throw new UnauthorizedAccessException("Estufa not found.");

        estufaRepository.Remove(estufa);
        await estufaRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> EnsureCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated.");
        if (await usuarioRepository.GetByIdAsync(userId, cancellationToken) is null)
            throw new UnauthorizedAccessException("User not found.");

        return userId;
    }

    private int? GetCurrentUserId()
    {
        var claimValue = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var parsed) ? parsed : null;
    }

    private bool BelongsToCurrentUser(Estufa estufa)
    {
        var currentUserId = GetCurrentUserId();
        return currentUserId is null || estufa.UsuarioId == currentUserId.Value;
    }

    private static EstufaResponseDto Map(Estufa estufa) => new(estufa.Id, estufa.Nome, estufa.Localizacao, estufa.UsuarioId);
}
