using System.Security.Claims;
using Aris.Application.DTOs.Culturas;
using Aris.Application.Helpers;
using Aris.Application.Interfaces.Repositories;
using Aris.Application.Services.Interfaces;
using Aris.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Aris.Application.Services.Implementations;

public class CulturaService(
    ICulturaRepository culturaRepository,
    IEstufaRepository estufaRepository,
    IParametroCulturaRepository parametroCulturaRepository,
    IHttpContextAccessor httpContextAccessor) : ICulturaService
{
    public async Task<List<CulturaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var estufasIds = await GetAllowedEstufaIdsAsync(currentUserId, cancellationToken);

        if (estufasIds.Count == 0)
        {
            return [];
        }

        var culturas = await culturaRepository.Query()
            .Where(x => estufasIds.Contains(x.EstufaId))
            .ToListAsync(cancellationToken);

        var culturasResponse = new List<CulturaResponseDto>(culturas.Count);
        foreach (var cultura in culturas)
        {
            var parametro = await parametroCulturaRepository.GetByCulturaIdAsync(cultura.Id, cancellationToken);
            culturasResponse.Add(Map(cultura, currentUserId, parametro));
        }

        return culturasResponse;
    }

    public async Task<CulturaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cultura = await culturaRepository.GetByIdAsync(id, cancellationToken);
        if (cultura is null || !await BelongsToCurrentUserAsync(cultura.EstufaId, cancellationToken))
            return null;

        var parametro = await parametroCulturaRepository.GetByCulturaIdAsync(id, cancellationToken);
        return Map(cultura, GetCurrentUserId(), parametro);
    }

    public async Task<CulturaResponseDto> CreateAsync(CreateCulturaDto request, CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var allowedEstufaIds = await GetAllowedEstufaIdsAsync(currentUserId, cancellationToken);

        if (allowedEstufaIds.Count == 0)
        {
            throw new InvalidOperationException("Cadastre uma estufa primeiro para criar uma cultura.");
        }

        if (!allowedEstufaIds.Contains(request.EstufaId))
        {
            throw new InvalidOperationException("Selecione uma estufa válida da sua conta.");
        }

        var cultura = new Cultura(request.Nome, request.EstufaId);
        EntityIdHelper.SetId(cultura, await EntityIdHelper.GetNextIdAsync(culturaRepository, cancellationToken));
        await culturaRepository.AddAsync(cultura, cancellationToken);
        await culturaRepository.SaveChangesAsync(cancellationToken);

        var parametro = new ParametroCultura(cultura.Id, request.TempMin, request.TempMax, request.UmidadeMin, request.UmidadeMax);
        EntityIdHelper.SetId(parametro, await EntityIdHelper.GetNextIdAsync(parametroCulturaRepository, cancellationToken));
        await parametroCulturaRepository.AddAsync(parametro, cancellationToken);
        await parametroCulturaRepository.SaveChangesAsync(cancellationToken);

        return Map(cultura, currentUserId, parametro);
    }

    public async Task<CulturaResponseDto> UpdateAsync(int id, UpdateCulturaDto request, CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var cultura = await culturaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Cultura not found.");

        if (!await BelongsToCurrentUserAsync(cultura.EstufaId, cancellationToken))
            throw new UnauthorizedAccessException("Cultura not found.");

        var allowedEstufaIds = await GetAllowedEstufaIdsAsync(currentUserId, cancellationToken);
        if (allowedEstufaIds.Count == 0)
        {
            throw new InvalidOperationException("Cadastre uma estufa primeiro para editar uma cultura.");
        }

        if (!allowedEstufaIds.Contains(request.EstufaId))
        {
            throw new InvalidOperationException("Selecione uma estufa válida da sua conta.");
        }

        cultura.Update(request.Nome, request.EstufaId);
        culturaRepository.Update(cultura);
        await culturaRepository.SaveChangesAsync(cancellationToken);

        var parametro = await parametroCulturaRepository.GetByCulturaIdAsync(id, cancellationToken);
        if (parametro is null)
        {
            parametro = new ParametroCultura(id, request.TempMin, request.TempMax, request.UmidadeMin, request.UmidadeMax);
            await parametroCulturaRepository.AddAsync(parametro, cancellationToken);
        }
        else
        {
            parametro.Update(id, request.TempMin, request.TempMax, request.UmidadeMin, request.UmidadeMax);
            parametroCulturaRepository.Update(parametro);
        }

        await parametroCulturaRepository.SaveChangesAsync(cancellationToken);
        return Map(cultura, GetCurrentUserId(), parametro);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cultura = await culturaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Cultura not found.");

        if (!await BelongsToCurrentUserAsync(cultura.EstufaId, cancellationToken))
            throw new UnauthorizedAccessException("Cultura not found.");

        var parametro = await parametroCulturaRepository.GetByCulturaIdAsync(id, cancellationToken);
        if (parametro is not null)
        {
            parametroCulturaRepository.Remove(parametro);
            await parametroCulturaRepository.SaveChangesAsync(cancellationToken);
        }

        culturaRepository.Remove(cultura);
        await culturaRepository.SaveChangesAsync(cancellationToken);
    }

    private int? GetCurrentUserId()
    {
        var claimValue = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var parsed) ? parsed : null;
    }

    private async Task<List<int>> GetAllowedEstufaIdsAsync(int? currentUserId, CancellationToken cancellationToken)
    {
        if (currentUserId is null)
        {
            return [];
        }

        return await estufaRepository.Query()
            .Where(x => x.UsuarioId == currentUserId.Value)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    private async Task<bool> BelongsToCurrentUserAsync(int estufaId, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            return true;

        var count = await estufaRepository.Query()
            .Where(x => x.Id == estufaId && x.UsuarioId == currentUserId.Value)
            .CountAsync(cancellationToken);

        return count > 0;
    }

    private static CulturaResponseDto Map(Cultura cultura, int? usuarioId = null, ParametroCultura? parametro = null)
    {
        return new CulturaResponseDto(
            cultura.Id,
            cultura.Nome,
            cultura.EstufaId,
            usuarioId ?? 0,
            parametro?.TempMin ?? cultura.ParametroCultura?.TempMin,
            parametro?.TempMax ?? cultura.ParametroCultura?.TempMax,
            parametro?.UmidadeMin ?? cultura.ParametroCultura?.UmidadeMin,
            parametro?.UmidadeMax ?? cultura.ParametroCultura?.UmidadeMax);
    }
}
