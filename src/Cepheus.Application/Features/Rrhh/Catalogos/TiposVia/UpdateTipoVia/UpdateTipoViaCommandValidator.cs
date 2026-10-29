using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.UpdateTipoVia
{
    public class UpdateTipoViaCommandValidator : AbstractValidator<UpdateTipoViaCommand>
    {
        public UpdateTipoViaCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de vía es obligatorio.")
                .MaximumLength(10).WithMessage("El código no puede exceder los 10 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de vía es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Abbreviation)
                .MaximumLength(20).WithMessage("La abreviatura no puede exceder los 20 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}