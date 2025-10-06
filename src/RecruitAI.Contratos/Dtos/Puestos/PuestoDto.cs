namespace RecruitAI.Contratos.Dtos.Puestos;

public class PuestoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Seniority { get; set; }
    public string? Ubicacion { get; set; }
    public IReadOnlyCollection<string> HabilidadesRequeridas { get; set; } = Array.Empty<string>();
    public DateTime CreadoEl { get; set; }
}
