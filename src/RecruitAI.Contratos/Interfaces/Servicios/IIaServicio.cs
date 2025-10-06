using RecruitAI.Contratos.Dtos.Ia;

namespace RecruitAI.Contratos.Interfaces.Servicios;

public interface IIaServicio
{
    Task<string> ExtraerCvAsync(ExtraerCvRequest request, CancellationToken cancellationToken = default);
    Task<double> PuntuarAsync(PuntuarRequest request, CancellationToken cancellationToken = default);
}
