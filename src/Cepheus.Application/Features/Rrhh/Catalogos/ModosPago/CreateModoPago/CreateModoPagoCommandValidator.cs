using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.CreateModoPago
{
    public class CreateModoPagoCommandValidator : AbstractValidator<CreateModoPagoCommand>
    {
        public CreateModoPagoCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del modo de pago es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}