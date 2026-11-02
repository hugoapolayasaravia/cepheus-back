using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.MarcarImpresionPagoProveedor
{
    public class MarcarImpresionPagoProveedorCommandValidator : AbstractValidator<MarcarImpresionPagoProveedorCommand>
    {
        public MarcarImpresionPagoProveedorCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de operación es obligatorio.");

            RuleFor(x => x.Tipo)
                .Must(t => string.Equals(t?.Trim(), "Retencion", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(t?.Trim(), "CajaBancos", StringComparison.OrdinalIgnoreCase))
                .WithMessage("El tipo de comprobante debe ser 'Retencion' o 'CajaBancos'.");
        }
    }
}
