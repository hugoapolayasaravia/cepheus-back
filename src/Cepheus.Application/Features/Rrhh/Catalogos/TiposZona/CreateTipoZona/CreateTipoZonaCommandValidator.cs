using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.CreateTipoZona
{
    public class CreateTipoZonaCommandValidator : AbstractValidator<CreateTipoZonaCommand>
    {
        public CreateTipoZonaCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de zona es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}