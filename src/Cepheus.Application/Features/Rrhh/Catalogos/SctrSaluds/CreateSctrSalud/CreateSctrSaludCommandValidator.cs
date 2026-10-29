using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.CreateSctrSalud
{
    public class CreateSctrSaludCommandValidator : AbstractValidator<CreateSctrSaludCommand>
    {
        public CreateSctrSaludCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la cobertura de salud SCTR es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}