using RecruitAI.Contratos.Entidades;

namespace RecruitAI.Datos.Entidades;

public class Puesto : IPuestoEntidad
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Seniority { get; set; }
    public string? Ubicacion { get; set; }
    public string? HabilidadesRequeridasJson { get; set; }
    public DateTime CreadoEl { get; set; } = DateTime.UtcNow;
    public string? Embedding { get; set; }

    public EmbeddingPuesto? EmbeddingPuesto { get; set; }
}
