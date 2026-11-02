using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.UpdatePagoProveedor
{
    public class UpdatePagoProveedorCommandValidator : PagoProveedorCabeceraValidator<UpdatePagoProveedorCommand>
    {
        public UpdatePagoProveedorCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de operación es obligatorio.")
                .MaximumLength(10).WithMessage("El código de operación no puede exceder los 10 caracteres.");
        }
    }
}
