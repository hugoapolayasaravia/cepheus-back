using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.CreateTipoAfiliacion
{
    public class CreateTipoAfiliacionCommandValidator : AbstractValidator<CreateTipoAfiliacionCommand>
    {
        public CreateTipoAfiliacionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de afiliación es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}