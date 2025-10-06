using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Entidades;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Datos.Repositorios;

public class PuestoRepositorio : RepositorioGenerico<Puesto, IPuestoEntidad>, IPuestoRepositorio
{
    public PuestoRepositorio(IDbContextFactory<CherokeeDbContext> contextoFactory)
        : base(contextoFactory)
    {
    }

    public async Task<IPuestoEntidad?> ObtenerConEmbeddingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var contexto = await _contextoFactory.CreateDbContextAsync(cancellationToken);
        var entidad = await contexto.Puestos
            .AsNoTracking()
            .Include(x => x.EmbeddingPuesto)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entidad;
    }
}
