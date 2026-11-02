using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.ActualizarPagoRealPagoProveedor
{
    public class ActualizarPagoRealPagoProveedorCommandValidator : AbstractValidator<ActualizarPagoRealPagoProveedorCommand>
    {
        public ActualizarPagoRealPagoProveedorCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de operación es obligatorio.");
            RuleFor(x => x.PagoRealSol).GreaterThanOrEqualTo(0).WithMessage("El monto pagado no puede ser negativo.");
        }
    }
}
