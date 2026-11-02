using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.AnularAsientoCancelacionVenta
{
    public class AnularAsientoCancelacionVentaCommandValidator : AbstractValidator<AnularAsientoCancelacionVentaCommand>
    {
        public AnularAsientoCancelacionVentaCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(200);
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
