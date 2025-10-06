using System.Linq;
using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Datos.Repositorios;

public class RepositorioGenerico<TEntity, TModelo> : IRepositorioGenerico<TModelo>
    where TEntity : class, TModelo
{
    protected readonly CherokeeDbContext ContextoEscritura;
    protected readonly IDbContextFactory<CherokeeDbContext> ContextoLecturaFactory;
    protected readonly DbSet<TEntity> ConjuntoEscritura;

    public RepositorioGenerico(
        CherokeeDbContext contextoEscritura,
        IDbContextFactory<CherokeeDbContext> contextoLecturaFactory)
    {
        ContextoEscritura = contextoEscritura;
        ContextoLecturaFactory = contextoLecturaFactory;
        ConjuntoEscritura = contextoEscritura.Set<TEntity>();
    }

    public virtual async Task<TModelo?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var contextoLectura = await ContextoLecturaFactory.CreateDbContextAsync(cancellationToken);
        var entidad = await contextoLectura.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken);
        return entidad;
    }

    public virtual async Task<List<TModelo>> ListarAsync(CancellationToken cancellationToken = default)
    {
        await using var contextoLectura = await ContextoLecturaFactory.CreateDbContextAsync(cancellationToken);
        var resultados = await contextoLectura.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return resultados.Cast<TModelo>().ToList();
    }

    public virtual async Task AgregarAsync(TModelo entidad, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        await ConjuntoEscritura.AddAsync((TEntity)entidad, cancellationToken);
    }

    public virtual Task ActualizarAsync(TModelo entidad, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        ConjuntoEscritura.Update((TEntity)entidad);
        return Task.CompletedTask;
    }

    public virtual Task EliminarAsync(TModelo entidad, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        ConjuntoEscritura.Remove((TEntity)entidad);
        return Task.CompletedTask;
    }

    public virtual Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        return ContextoEscritura.SaveChangesAsync(cancellationToken);
    }
}
