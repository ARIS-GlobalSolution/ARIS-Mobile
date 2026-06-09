using Aris.Application.Helpers;
using Aris.Application.Interfaces.Repositories;
using Aris.Application.Services.Implementations;
using Aris.Domain.Entities;
using Aris.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aris.Infrastructure.Services;

public class ArisSeedService(
    IRepository<Usuario> usuarioRepository,
    IRepository<TipoSensor> tipoSensorRepository,
    IRepository<Estufa> estufaRepository,
    IRepository<Cultura> culturaRepository,
    ArisDbContext context)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await usuarioRepository.Query().CountAsync(cancellationToken) == 0)
        {
            var admin = new Usuario("Admin ARIS", "admin@aris.local", AuthService.HashPassword("Admin@123"));
            EntityIdHelper.SetId(admin, 1);
            await usuarioRepository.AddAsync(admin, cancellationToken);
            await usuarioRepository.SaveChangesAsync(cancellationToken);
        }

        if (await tipoSensorRepository.Query().CountAsync(cancellationToken) == 0)
        {
            var temperatura = new TipoSensor("Temperatura", "C");
            EntityIdHelper.SetId(temperatura, 1);
            await tipoSensorRepository.AddAsync(temperatura, cancellationToken);

            var umidade = new TipoSensor("Umidade", "%");
            EntityIdHelper.SetId(umidade, 2);
            await tipoSensorRepository.AddAsync(umidade, cancellationToken);

            var luminosidade = new TipoSensor("Luminosidade", "lux");
            EntityIdHelper.SetId(luminosidade, 3);
            await tipoSensorRepository.AddAsync(luminosidade, cancellationToken);

            await tipoSensorRepository.SaveChangesAsync(cancellationToken);
        }

        if (await estufaRepository.Query().CountAsync(cancellationToken) == 0)
        {
            var usuario = await usuarioRepository.Query().FirstAsync(cancellationToken);
            var estufa = new Estufa("Estufa ARIS 01", "Laboratorio", usuario.Id);
            EntityIdHelper.SetId(estufa, 1);
            await estufaRepository.AddAsync(estufa, cancellationToken);
            await estufaRepository.SaveChangesAsync(cancellationToken);

            var cultura = new Cultura("Alface", estufa.Id);
            EntityIdHelper.SetId(cultura, 1);
            await culturaRepository.AddAsync(cultura, cancellationToken);
            await culturaRepository.SaveChangesAsync(cancellationToken);

            var parametro = new ParametroCultura(cultura.Id, 10, 25, 60, 80);
            EntityIdHelper.SetId(parametro, 1);
            await context.Set<ParametroCultura>().AddAsync(parametro, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
