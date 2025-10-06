using RecruitAI.Contratos.Entidades;

namespace RecruitAI.Datos.Entidades;

public class EmbeddingPuesto : IEmbeddingEntidad
{
    public Guid PuestoId { get; set; }
    public byte[]? Vector { get; set; }
    public string Modelo { get; set; } = "text-embedding-3-large";
    public DateTime ActualizadoEl { get; set; } = DateTime.UtcNow;

    public Puesto Puesto { get; set; } = null!;
}
