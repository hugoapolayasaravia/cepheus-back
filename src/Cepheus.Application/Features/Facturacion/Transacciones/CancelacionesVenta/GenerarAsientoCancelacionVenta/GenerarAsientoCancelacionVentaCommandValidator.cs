using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GenerarAsientoCancelacionVenta
{
    public class GenerarAsientoCancelacionVentaCommandValidator : AbstractValidator<GenerarAsientoCancelacionVentaCommand>
    {
        public GenerarAsientoCancelacionVentaCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
