namespace RecruitAI.Contratos.Dtos.Candidatos;

public class CrearCandidatoDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Fuente { get; set; }
    public string CvTexto { get; set; } = string.Empty;
}
