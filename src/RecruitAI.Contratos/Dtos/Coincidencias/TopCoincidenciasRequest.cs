namespace RecruitAI.Contratos.Dtos.Coincidencias;

public class TopCoincidenciasRequest
{
    public int Cantidad { get; set; } = 20;
    public double? Umbral { get; set; }
}
