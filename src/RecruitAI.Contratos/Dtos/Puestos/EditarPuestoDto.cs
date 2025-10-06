namespace RecruitAI.Contratos.Dtos.Puestos;

public class EditarPuestoDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Seniority { get; set; }
    public string? Ubicacion { get; set; }
    public List<string> HabilidadesRequeridas { get; set; } = new();
}
