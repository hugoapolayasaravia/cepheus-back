using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.CuadrarCancelacionVenta
{
    public class CuadrarCancelacionVentaCommandValidator : AbstractValidator<CuadrarCancelacionVentaCommand>
    {
        public CuadrarCancelacionVentaCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Item).NotEmpty().MaximumLength(3);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio.");
        }
    }
}
