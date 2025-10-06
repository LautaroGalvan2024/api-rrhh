namespace RecruitAI.Contratos.Dtos.Autenticacion;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEn { get; set; }
        = DateTime.UtcNow;
    public string Rol { get; set; } = string.Empty;
}
