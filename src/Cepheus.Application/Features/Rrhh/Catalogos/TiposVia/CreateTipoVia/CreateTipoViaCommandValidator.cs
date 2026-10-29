using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.CreateTipoVia
{
    public class CreateTipoViaCommandValidator : AbstractValidator<CreateTipoViaCommand>
    {
        public CreateTipoViaCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de vía es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Abbreviation)
                .MaximumLength(20).WithMessage("La abreviatura no puede exceder los 20 caracteres.");
        }
    }
}