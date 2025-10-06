using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Entidades;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Datos.Repositorios;

public class CandidatoRepositorio : RepositorioGenerico<Candidato, ICandidatoEntidad>, ICandidatoRepositorio
{
    public CandidatoRepositorio(IDbContextFactory<CherokeeDbContext> contextoFactory)
        : base(contextoFactory)
    {
    }

    public async Task<ICandidatoEntidad?> ObtenerConEmbeddingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var contexto = await _contextoFactory.CreateDbContextAsync(cancellationToken);
        var entidad = await contexto.Candidatos
            .AsNoTracking()
            .Include(x => x.EmbeddingCandidato)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entidad;
    }

    public async Task<List<ICandidatoEntidad>> ListarConEmbeddingAsync(CancellationToken cancellationToken = default)
    {
        await using var contexto = await _contextoFactory.CreateDbContextAsync(cancellationToken);
        var entidades = await contexto.Candidatos
            .AsNoTracking()
            .Include(x => x.EmbeddingCandidato)
            .ToListAsync(cancellationToken);
        return entidades.Cast<ICandidatoEntidad>().ToList();
    }
}
