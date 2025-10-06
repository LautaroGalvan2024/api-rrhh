using RecruitAI.Contratos.Entidades;

namespace RecruitAI.Datos.Entidades;

public class EmbeddingCandidato : IEmbeddingEntidad
{
    public Guid CandidatoId { get; set; }
    public byte[]? Vector { get; set; }
    public string Modelo { get; set; } = "text-embedding-3-large";
    public DateTime ActualizadoEl { get; set; } = DateTime.UtcNow;

    public Candidato Candidato { get; set; } = null!;
}
