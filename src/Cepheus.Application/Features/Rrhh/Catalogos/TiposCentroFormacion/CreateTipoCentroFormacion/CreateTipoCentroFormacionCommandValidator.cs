using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.CreateTipoCentroFormacion
{
    public class CreateTipoCentroFormacionCommandValidator : AbstractValidator<CreateTipoCentroFormacionCommand>
    {
        public CreateTipoCentroFormacionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de centro de formación es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}