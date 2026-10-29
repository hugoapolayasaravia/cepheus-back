using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.CreateOrdenTrabajoVenta;

public sealed class CreateOrdenTrabajoVentaCommandValidator : OrdenTrabajoVentaFieldsValidator<CreateOrdenTrabajoVentaCommand>
{
    public CreateOrdenTrabajoVentaCommandValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5);
        RuleFor(x => x.ObraCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.OriginCode).MaximumLength(8);

        RuleFor(x => x.CotizacionNegocioCode).MaximumLength(2);
        RuleFor(x => x.CotizacionYear).MaximumLength(4);
        RuleFor(x => x.CotizacionMonth).MaximumLength(2);
        RuleFor(x => x.CotizacionCode).MaximumLength(10);

        RuleFor(x => x)
            .Must(x =>
            {
                var parts = new[] { x.CotizacionNegocioCode, x.CotizacionYear, x.CotizacionMonth, x.CotizacionCode };
                var filled = parts.Count(p => !string.IsNullOrWhiteSpace(p));
                return filled == 0 || filled == 4;
            })
            .WithMessage("La cotización se indica completa (negocio, año, mes y código) o no se indica.");

        RuleForEach(x => x.Detalles).SetValidator(new OrdenTrabajoVentaDetalleRequestValidator());
    }
}
