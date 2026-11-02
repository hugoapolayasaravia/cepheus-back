using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GenerarAsientoPagoProveedor
{
    public class GenerarAsientoPagoProveedorCommandValidator : AbstractValidator<GenerarAsientoPagoProveedorCommand>
    {
        public GenerarAsientoPagoProveedorCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de operación es obligatorio.");
        }
    }
}
