namespace RecruitAI.Contratos.Interfaces.Repositorios;

public interface IRepositorioGenerico<T>
{
    Task<T?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<T>> ListarAsync(CancellationToken cancellationToken = default);
    Task AgregarAsync(T entidad, CancellationToken cancellationToken = default);
    Task ActualizarAsync(T entidad, CancellationToken cancellationToken = default);
    Task EliminarAsync(T entidad, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
