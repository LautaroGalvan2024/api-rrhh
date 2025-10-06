using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Entidades;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Datos.Repositorios;

public class CandidatoRepositorio : RepositorioGenerico<Candidato, ICandidatoEntidad>, ICandidatoRepositorio
{
    public CandidatoRepositorio(CherokeeDbContext context) : base(context)
    {
    }

    public async Task<ICandidatoEntidad?> ObtenerConEmbeddingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entidad = await Context.Candidatos
            .Include(x => x.EmbeddingCandidato)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entidad;
    }

    public async Task<List<ICandidatoEntidad>> ListarConEmbeddingAsync(CancellationToken cancellationToken = default)
    {
        var entidades = await Context.Candidatos
            .Include(x => x.EmbeddingCandidato)
            .ToListAsync(cancellationToken);
        return entidades.Cast<ICandidatoEntidad>().ToList();
    }
}
