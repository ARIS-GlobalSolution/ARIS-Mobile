using Aris.Application.Interfaces.Repositories;
using Aris.Application.Services.Implementations;
using Aris.Application.Services.Interfaces;
using Aris.Infrastructure.Persistence;
using Aris.Infrastructure.Repositories;
using Aris.Infrastructure.Services;
using Aris.api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("ArisOracle")
    ?? builder.Configuration.GetConnectionString("OracleConnection")
    ?? builder.Configuration.GetConnectionString("Oracle")
    ?? throw new InvalidOperationException("Connection string not configured.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Jwt:Issuer"] ?? "ARIS";
        var audience = builder.Configuration["Jwt:Audience"] ?? "ARIS";
        var key = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ArisDbContext>(options =>
    options.UseOracle(connectionString));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEstufaService, EstufaService>();
builder.Services.AddScoped<ICulturaService, CulturaService>();
builder.Services.AddScoped<ISensorService, SensorService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEstufaRepository, EstufaRepository>();
builder.Services.AddScoped<ICulturaRepository, CulturaRepository>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ITelemetriaRepository, TelemetriaRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IIrrigacaoRepository, IrrigacaoRepository>();
builder.Services.AddScoped<ILogAcaoRepository, LogAcaoRepository>();
builder.Services.AddScoped<IParametroCulturaRepository, ParametroCulturaRepository>();
builder.Services.AddScoped<ITipoSensorRepository, TipoSensorRepository>();

builder.Services.AddSingleton<TelemetrySnapshotStore>();
builder.Services.AddHostedService<MqttTelemetryHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
