using RecruitAI.Contratos.Dtos.Autenticacion;

namespace RecruitAI.Contratos.Interfaces.Servicios;

public interface IAuthServicio
{
    Task<TokenResponse> GenerarTokenAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
