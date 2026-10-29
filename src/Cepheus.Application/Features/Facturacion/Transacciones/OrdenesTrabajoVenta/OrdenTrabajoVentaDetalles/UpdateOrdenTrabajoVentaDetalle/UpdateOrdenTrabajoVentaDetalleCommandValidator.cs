using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.UpdateOrdenTrabajoVentaDetalle;

public sealed class UpdateOrdenTrabajoVentaDetalleCommandValidator : AbstractValidator<UpdateOrdenTrabajoVentaDetalleCommand>
{
    public UpdateOrdenTrabajoVentaDetalleCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Item).GreaterThan(0);
        RuleFor(x => x.Data).NotNull().SetValidator(new UpdateOrdenTrabajoVentaDetalleRequestValidator());
    }
}
