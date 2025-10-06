using Microsoft.EntityFrameworkCore;
using RecruitAI.Contratos.Interfaces.Repositorios;
using RecruitAI.Datos.Persistencia;

namespace RecruitAI.Datos.Repositorios;

public class RepositorioGenerico<TEntity, TModelo> : IRepositorioGenerico<TModelo>
    where TEntity : class, TModelo
{
    protected readonly IDbContextFactory<CherokeeDbContext> _contextoFactory;

    public RepositorioGenerico(IDbContextFactory<CherokeeDbContext> contextoFactory)
    {
        _contextoFactory = contextoFactory;
    }

    // Obtener por ID
    public virtual async Task<TModelo?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var contexto = await _contextoFactory.CreateDbContextAsync(cancellationToken);
        return await contexto.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken);
    }

    // Listar todos
    public virtual async Task<List<TModelo>> ListarAsync(CancellationToken cancellationToken = default)
    {
        await using var contexto = await _contextoFactory.CreateDbContextAsync(cancellationToken);
        var resultados = await contexto.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return resultados.Cast<TModelo>().ToList();
    }

    // Agregar
    public virtual async Task AgregarAsync(TModelo entidad, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidad);

        await using var contexto = await _contextoFactory.CreateDbContextAsync(cancellationToken);
        await contexto.Set<TEntity>().AddAsync((TEntity)entidad, cancellationToken);
        await contexto.SaveChangesAsync(cancellationToken);
    }

    // Actualizar
    public virtual async Task ActualizarAsync(TModelo entidad, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidad);

        await using var contexto = await _contextoFactory.CreateDbContextAsync(cancellationToken);
        contexto.Set<TEntity>().Update((TEntity)entidad);
        await contexto.SaveChangesAsync(cancellationToken);
    }

    // Eliminar
    public virtual async Task EliminarAsync(TModelo entidad, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidad);

        await using var contexto = await _contextoFactory.CreateDbContextAsync(cancellationToken);
        contexto.Set<TEntity>().Remove((TEntity)entidad);
        await contexto.SaveChangesAsync(cancellationToken);
    }

    // Guardar cambios (opcional)
    // Si quieres mantener el método por compatibilidad de interfaz
    public virtual async Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        await using var contexto = await _contextoFactory.CreateDbContextAsync(cancellationToken);
        await contexto.SaveChangesAsync(cancellationToken);
    }
}
