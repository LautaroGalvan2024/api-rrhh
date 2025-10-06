namespace RecruitAI.Contratos.Dtos.Candidatos;

public class CandidatoDto
{
    public Guid Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Fuente { get; set; }
    public string CvTexto { get; set; } = string.Empty;
    public DateTime CreadoEl { get; set; }
}
