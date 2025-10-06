namespace RecruitAI.Contratos.Configuracion;

public class JwtOptions
{
    public string Emisor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public string Secreto { get; set; } = string.Empty;
    public int ExpiracionMinutos { get; set; } = 60;
}
