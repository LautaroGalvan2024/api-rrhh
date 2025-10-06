using FluentValidation;
using RecruitAI.Contratos.Dtos.Puestos;

namespace RecruitAI.Contratos.Validaciones.Puestos;

public class CrearPuestoValidador : AbstractValidator<CrearPuestoDto>
{
    public CrearPuestoValidador()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(120).WithMessage("El título no puede superar 120 caracteres.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MinimumLength(30).WithMessage("La descripción debe tener al menos 30 caracteres.");

        RuleForEach(x => x.HabilidadesRequeridas)
            .MaximumLength(60).WithMessage("Cada habilidad no puede superar 60 caracteres.");
    }
}
