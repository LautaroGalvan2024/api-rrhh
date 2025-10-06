namespace RecruitAI.Contratos.Dtos.Coincidencias;

public class CoincidenciaDto
{
    public Guid CandidatoId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public double Puntaje { get; set; }
}
