using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.CreateSctrPension
{
    public class CreateSctrPensionCommandValidator : AbstractValidator<CreateSctrPensionCommand>
    {
        public CreateSctrPensionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la cobertura de pensión SCTR es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}