namespace RecruitAI.Contratos.Entidades;

public interface IPuestoEntidad
{
    Guid Id { get; }
    string Titulo { get; }
    string Descripcion { get; }
    string? Seniority { get; }
    string? Ubicacion { get; }
    string? HabilidadesRequeridasJson { get; }
    DateTime CreadoEl { get; }
}
