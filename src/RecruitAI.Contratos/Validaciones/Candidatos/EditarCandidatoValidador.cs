using FluentValidation;
using RecruitAI.Contratos.Dtos.Candidatos;

namespace RecruitAI.Contratos.Validaciones.Candidatos;

public class EditarCandidatoValidador : AbstractValidator<EditarCandidatoDto>
{
    public EditarCandidatoValidador()
    {
        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("El correo debe tener un formato válido.");

        RuleFor(x => x.CvTexto)
            .NotEmpty().WithMessage("El CV no puede estar vacío.");
    }
}
