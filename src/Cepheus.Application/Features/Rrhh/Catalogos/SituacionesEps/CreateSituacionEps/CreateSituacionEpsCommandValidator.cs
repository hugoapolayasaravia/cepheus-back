using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.CreateSituacionEps
{
    public class CreateSituacionEpsCommandValidator : AbstractValidator<CreateSituacionEpsCommand>
    {
        public CreateSituacionEpsCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la situación EPS es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}