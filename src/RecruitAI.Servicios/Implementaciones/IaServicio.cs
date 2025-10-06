using RecruitAI.Contratos.Dtos.Ia;
using RecruitAI.Contratos.Interfaces.Servicios;

namespace RecruitAI.Servicios.Implementaciones;

public class IaServicio : IIaServicio
{
    public Task<string> ExtraerCvAsync(ExtraerCvRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("Funcionalidad de extracción pendiente de implementar.");
    }

    public Task<double> PuntuarAsync(PuntuarRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0d);
    }
}
