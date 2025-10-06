using RecruitAI.Contratos.Entidades;

namespace RecruitAI.Datos.Entidades;

public class Candidato : ICandidatoEntidad
{
    public Guid Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Fuente { get; set; }
    public string CvTexto { get; set; } = string.Empty;
    public DateTime CreadoEl { get; set; } = DateTime.UtcNow;
    public string? Embedding { get; set; }

    public EmbeddingCandidato? EmbeddingCandidato { get; set; }
}
