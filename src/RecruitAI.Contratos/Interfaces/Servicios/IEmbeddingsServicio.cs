namespace RecruitAI.Contratos.Interfaces.Servicios;

public interface IEmbeddingsServicio
{
    Task<float[]> GenerarEmbeddingAsync(string texto, string modelo = "text-embedding-3-large", CancellationToken cancellationToken = default);
    byte[] ConvertirABytes(float[] vector);
    float[] ConvertirAFlotantes(byte[] datos);
}
