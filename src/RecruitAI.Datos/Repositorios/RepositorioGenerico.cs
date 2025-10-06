using System.Linq;
using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Datos.Repositorios;

public class RepositorioGenerico<TEntity, TModelo> : IRepositorioGenerico<TModelo>
    where TEntity : class, TModelo
{
    protected readonly CherokeeDbContext Context;
    protected readonly DbSet<TEntity> Conjunto;

    public RepositorioGenerico(CherokeeDbContext context)
    {
        Context = context;
        Conjunto = context.Set<TEntity>();
    }

    public virtual async Task<TModelo?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entidad = await Conjunto.FindAsync(new object?[] { id }, cancellationToken);
        return entidad;
    }

    public virtual async Task<List<TModelo>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var resultados = await Conjunto.ToListAsync(cancellationToken);
        return resultados.Cast<TModelo>().ToList();
    }

    public virtual async Task AgregarAsync(TModelo entidad, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        await Conjunto.AddAsync((TEntity)entidad, cancellationToken);
    }

    public virtual Task ActualizarAsync(TModelo entidad, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        Conjunto.Update((TEntity)entidad);
        return Task.CompletedTask;
    }

    public virtual Task EliminarAsync(TModelo entidad, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        Conjunto.Remove((TEntity)entidad);
        return Task.CompletedTask;
    }

    public virtual Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}
