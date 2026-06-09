using Aris.Application.Interfaces.Repositories;
using Aris.Application.Services.Implementations;
using Aris.Application.Services.Interfaces;
using Aris.Infrastructure.Persistence;
using Aris.Infrastructure.Repositories;
using Aris.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Aris.api.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IEstufaService, EstufaService>();
        services.AddScoped<ISensorService, SensorService>();
        services.AddScoped<ICulturaService, CulturaService>();
        services.AddScoped<ArisSeedService>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IEstufaRepository, EstufaRepository>();
        services.AddScoped<ITipoSensorRepository, TipoSensorRepository>();
        services.AddScoped<ISensorRepository, SensorRepository>();
        services.AddScoped<ICulturaRepository, CulturaRepository>();
        services.AddScoped<IParametroCulturaRepository, ParametroCulturaRepository>();
        services.AddScoped<ITelemetriaRepository, TelemetriaRepository>();
        services.AddScoped<IIrrigacaoRepository, IrrigacaoRepository>();
        services.AddScoped<IAlertaRepository, AlertaRepository>();
        services.AddScoped<ILogAcaoRepository, LogAcaoRepository>();

        return services;
    }

    public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ArisDbContext>(options =>
        {
            options.UseOracle(configuration.GetConnectionString("ArisOracle"));
        });

        return services;
    }
}
