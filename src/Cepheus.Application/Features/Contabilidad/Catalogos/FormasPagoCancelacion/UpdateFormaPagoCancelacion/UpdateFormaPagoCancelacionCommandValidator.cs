using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.UpdateFormaPagoCancelacion
{
    public class UpdateFormaPagoCancelacionCommandValidator : AbstractValidator<UpdateFormaPagoCancelacionCommand>
    {
        public UpdateFormaPagoCancelacionCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(1).WithMessage("El código debe tener 1 carácter.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(50).WithMessage("La descripción no puede exceder los 50 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
