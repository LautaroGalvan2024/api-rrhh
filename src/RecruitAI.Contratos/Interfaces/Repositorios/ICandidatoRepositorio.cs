using RecruitAI.Contratos.Entidades;

namespace RecruitAI.Contratos.Interfaces.Repositorios;

public interface ICandidatoRepositorio : IRepositorioGenerico<ICandidatoEntidad>
{
    Task<ICandidatoEntidad?> ObtenerConEmbeddingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ICandidatoEntidad>> ListarConEmbeddingAsync(CancellationToken cancellationToken = default);
}
