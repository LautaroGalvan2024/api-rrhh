using Microsoft.EntityFrameworkCore;
using RecruitAI.Datos.Entidades;

namespace RecruitAI.Datos.Persistencia;

public class CherokeeDbContext : DbContext
{
    public CherokeeDbContext(DbContextOptions<CherokeeDbContext> options) : base(options)
    {
    }

    public DbSet<Puesto> Puestos => Set<Puesto>();
    public DbSet<Candidato> Candidatos => Set<Candidato>();
    public DbSet<EmbeddingPuesto> EmbeddingsPuestos => Set<EmbeddingPuesto>();
    public DbSet<EmbeddingCandidato> EmbeddingsCandidatos => Set<EmbeddingCandidato>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Puesto>(entity =>
        {
            entity.ToTable("Puestos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).HasMaxLength(200);
            entity.Property(e => e.Seniority).HasMaxLength(100);
            entity.Property(e => e.Ubicacion).HasMaxLength(150);
            entity.Property(e => e.HabilidadesRequeridasJson).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Embedding).HasColumnType("nvarchar(max)");
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.EmbeddingPuesto)
                .WithOne(e => e.Puesto)
                .HasForeignKey<EmbeddingPuesto>(e => e.PuestoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Candidato>(entity =>
        {
            entity.ToTable("Candidatos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreCompleto).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Fuente).HasMaxLength(150);
            entity.Property(e => e.Embedding).HasColumnType("nvarchar(max)");
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.EmbeddingCandidato)
                .WithOne(e => e.Candidato)
                .HasForeignKey<EmbeddingCandidato>(e => e.CandidatoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmbeddingPuesto>(entity =>
        {
            entity.ToTable("EmbeddingsPuestos");
            entity.HasKey(e => e.PuestoId);
            entity.Property(e => e.Vector).HasColumnType("varbinary(max)");
            entity.Property(e => e.Modelo).HasMaxLength(200);
        });

        modelBuilder.Entity<EmbeddingCandidato>(entity =>
        {
            entity.ToTable("EmbeddingsCandidatos");
            entity.HasKey(e => e.CandidatoId);
            entity.Property(e => e.Vector).HasColumnType("varbinary(max)");
            entity.Property(e => e.Modelo).HasMaxLength(200);
        });
    }
}
