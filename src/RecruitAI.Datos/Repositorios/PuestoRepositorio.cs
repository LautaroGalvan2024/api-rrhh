using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Entidades;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Datos.Repositorios;

public class PuestoRepositorio : RepositorioGenerico<Puesto, IPuestoEntidad>, IPuestoRepositorio
{
    public PuestoRepositorio(
        CherokeeDbContext contextoEscritura,
        IDbContextFactory<CherokeeDbContext> contextoLecturaFactory) : base(contextoEscritura, contextoLecturaFactory)
    {
    }

    public async Task<IPuestoEntidad?> ObtenerConEmbeddingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var contextoLectura = await ContextoLecturaFactory.CreateDbContextAsync(cancellationToken);
        var entidad = await contextoLectura.Puestos
            .AsNoTracking()
            .Include(x => x.EmbeddingPuesto)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entidad;
    }
}
