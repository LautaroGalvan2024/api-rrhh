namespace RecruitAI.Contratos.Entidades;

public interface IEmbeddingEntidad
{
    byte[]? Vector { get; }
    string Modelo { get; }
    DateTime ActualizadoEl { get; }
}
