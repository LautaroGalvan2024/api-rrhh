using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Entidades;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Datos.Entidades;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Datos.Repositorios;

public class PuestoRepositorio : RepositorioGenerico<Puesto, IPuestoEntidad>, IPuestoRepositorio
{
    public PuestoRepositorio(CherokeeDbContext context) : base(context)
    {
    }

    public async Task<IPuestoEntidad?> ObtenerConEmbeddingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entidad = await Context.Puestos
            .Include(x => x.EmbeddingPuesto)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entidad;
    }
}
