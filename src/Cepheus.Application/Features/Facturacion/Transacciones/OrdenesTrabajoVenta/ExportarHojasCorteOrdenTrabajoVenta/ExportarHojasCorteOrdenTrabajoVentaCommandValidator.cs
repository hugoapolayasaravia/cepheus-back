using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.ExportarHojasCorteOrdenTrabajoVenta;

public sealed class ExportarHojasCorteOrdenTrabajoVentaCommandValidator
    : AbstractValidator<ExportarHojasCorteOrdenTrabajoVentaCommand>
{
    public ExportarHojasCorteOrdenTrabajoVentaCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data is not null, () =>
        {
            RuleFor(x => x.Data.Hojas).NotEmpty().WithMessage("Indique al menos una Hoja de Corte.");

            RuleForEach(x => x.Data.Hojas).ChildRules(h =>
            {
                h.RuleFor(y => y.Year).NotEmpty().Length(4);
                h.RuleFor(y => y.Code).NotEmpty().MaximumLength(8);
            });

            RuleForEach(x => x.Data.Precios).ChildRules(p =>
            {
                p.RuleFor(y => y.ProductoTipoCode).NotEmpty().MaximumLength(2);
                p.RuleFor(y => y.ProductoCode).NotEmpty().MaximumLength(4);
                p.RuleFor(y => y.UnitPrice).GreaterThan(0).WithMessage("El precio debe ser mayor a cero.");
            });
        });
    }
}
