namespace RecruitAI.Contratos.Entidades;

public interface ICandidatoEntidad
{
    Guid Id { get; }
    string NombreCompleto { get; }
    string Email { get; }
    string? Fuente { get; }
    string CvTexto { get; }
    DateTime CreadoEl { get; }
}
