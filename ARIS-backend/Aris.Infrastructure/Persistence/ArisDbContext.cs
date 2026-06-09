using Aris.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aris.Infrastructure.Persistence;

public class ArisDbContext(DbContextOptions<ArisDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Estufa> Estufas => Set<Estufa>();
    public DbSet<TipoSensor> TiposSensor => Set<TipoSensor>();
    public DbSet<Sensor> Sensores => Set<Sensor>();
    public DbSet<Telemetria> Telemetrias => Set<Telemetria>();
    public DbSet<Cultura> Culturas => Set<Cultura>();
    public DbSet<ParametroCultura> ParametrosCultura => Set<ParametroCultura>();
    public DbSet<Irrigacao> Irrigacoes => Set<Irrigacao>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<LogAcao> LogAcoes => Set<LogAcao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("USUARIOS");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_USUARIO").ValueGeneratedNever();
            entity.Property(x => x.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasColumnName("EMAIL").HasMaxLength(100).IsRequired();
            entity.Property(x => x.SenhaHash).HasColumnName("SENHA").HasMaxLength(100).IsRequired();
            entity.Property(x => x.DataCadastro).HasColumnName("DATA_CADASTRO");
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Estufa>(entity =>
        {
            entity.ToTable("ESTUFAS");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_ESTUFA").ValueGeneratedNever();
            entity.Property(x => x.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Localizacao).HasColumnName("LOCALIZACAO").HasMaxLength(100);
            entity.Property(x => x.UsuarioId).HasColumnName("ID_USUARIO");
            entity.HasOne(x => x.Usuario).WithMany(x => x.Estufas).HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TipoSensor>(entity =>
        {
            entity.ToTable("TIPOS_SENSOR");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_TIPO").ValueGeneratedNever();
            entity.Property(x => x.Nome).HasColumnName("NOME").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Unidade).HasColumnName("UNIDADE").HasMaxLength(20);
        });

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.ToTable("SENSORES");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_SENSOR").ValueGeneratedNever();
            entity.Property(x => x.TipoSensorId).HasColumnName("ID_TIPO");
            entity.Property(x => x.EstufaId).HasColumnName("ID_ESTUFA");
            entity.HasOne(x => x.TipoSensor).WithMany(x => x.Sensores).HasForeignKey(x => x.TipoSensorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Estufa).WithMany(x => x.Sensores).HasForeignKey(x => x.EstufaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Telemetria>(entity =>
        {
            entity.ToTable("TELEMETRIA");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_TELEMETRIA").ValueGeneratedNever();
            entity.Property(x => x.Valor).HasColumnName("VALOR").HasPrecision(10, 2).IsRequired();
            entity.Property(x => x.DataHora).HasColumnName("DATA_HORA");
            entity.Property(x => x.SensorId).HasColumnName("ID_SENSOR");
            entity.HasOne(x => x.Sensor).WithMany(x => x.Telemetrias).HasForeignKey(x => x.SensorId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Cultura>(entity =>
        {
            entity.ToTable("CULTURAS");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_CULTURA").ValueGeneratedNever();
            entity.Property(x => x.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            entity.Property(x => x.EstufaId).HasColumnName("ID_ESTUFA");
            entity.HasOne(x => x.Estufa).WithMany(x => x.Culturas).HasForeignKey(x => x.EstufaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ParametroCultura>(entity =>
        {
            entity.ToTable("PARAMETROS_CULTURA");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_PARAMETRO").ValueGeneratedNever();
            entity.Property(x => x.CulturaId).HasColumnName("ID_CULTURA");
            entity.Property(x => x.TempMin).HasColumnName("TEMP_MIN").HasPrecision(10, 2);
            entity.Property(x => x.TempMax).HasColumnName("TEMP_MAX").HasPrecision(10, 2);
            entity.Property(x => x.UmidadeMin).HasColumnName("UMIDADE_MIN").HasPrecision(10, 2);
            entity.Property(x => x.UmidadeMax).HasColumnName("UMIDADE_MAX").HasPrecision(10, 2);
            entity.HasOne(x => x.Cultura).WithOne(x => x.ParametroCultura).HasForeignKey<ParametroCultura>(x => x.CulturaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Irrigacao>(entity =>
        {
            entity.ToTable("IRRIGACAO");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_IRRIGACAO").ValueGeneratedNever();
            entity.Property(x => x.DataHora).HasColumnName("DATA_HORA");
            entity.Property(x => x.Status).HasColumnName("STATUS").HasMaxLength(20).IsRequired();
            entity.Property(x => x.EstufaId).HasColumnName("ID_ESTUFA");
            entity.HasOne(x => x.Estufa).WithMany(x => x.Irrigacoes).HasForeignKey(x => x.EstufaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Alerta>(entity =>
        {
            entity.ToTable("ALERTAS");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_ALERTA").ValueGeneratedNever();
            entity.Property(x => x.Mensagem).HasColumnName("MENSAGEM").HasMaxLength(200);
            entity.Property(x => x.NivelRisco).HasColumnName("NIVEL_RISCO").HasMaxLength(20).IsRequired();
            entity.Property(x => x.DataHora).HasColumnName("DATA_HORA");
            entity.Property(x => x.EstufaId).HasColumnName("ID_ESTUFA");
            entity.HasOne(x => x.Estufa).WithMany(x => x.Alertas).HasForeignKey(x => x.EstufaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LogAcao>(entity =>
        {
            entity.ToTable("LOG_ACOES");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID_LOG").ValueGeneratedNever();
            entity.Property(x => x.Acao).HasColumnName("ACAO").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Descricao).HasColumnName("DESCRICAO").HasMaxLength(200).IsRequired();
            entity.Property(x => x.DataHora).HasColumnName("DATA_HORA");
        });

        base.OnModelCreating(modelBuilder);
    }
}
