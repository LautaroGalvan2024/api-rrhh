using RecruitAI.Contratos.Dtos.Coincidencias;

namespace RecruitAI.Contratos.Interfaces.Servicios;

public interface ICoincidenciasServicio
{
    Task<IReadOnlyCollection<CoincidenciaDto>> TopPorPuestoAsync(Guid puestoId, int cantidad = 20, double? umbral = null, CancellationToken cancellationToken = default);
}
