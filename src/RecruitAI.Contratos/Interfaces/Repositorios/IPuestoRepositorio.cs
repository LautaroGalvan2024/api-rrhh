using RecruitAI.Contratos.Entidades;

namespace RecruitAI.Contratos.Interfaces.Repositorios;

public interface IPuestoRepositorio : IRepositorioGenerico<IPuestoEntidad>
{
    Task<IPuestoEntidad?> ObtenerConEmbeddingAsync(Guid id, CancellationToken cancellationToken = default);
}
