namespace RecruitAI.Contratos.Configuracion;

public class DatabaseOptions
{
    public string Provider { get; set; } = "SqlServer";
    public string? ConnectionString { get; set; }
    public string? ReadOnlyConnectionString { get; set; }
}
