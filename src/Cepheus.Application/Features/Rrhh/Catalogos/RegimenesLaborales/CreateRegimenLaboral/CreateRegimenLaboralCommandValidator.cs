using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.CreateRegimenLaboral
{
    public class CreateRegimenLaboralCommandValidator : AbstractValidator<CreateRegimenLaboralCommand>
    {
        public CreateRegimenLaboralCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del régimen laboral es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}