using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.DeleteOrdenTrabajoVentaDetalle;

public sealed class DeleteOrdenTrabajoVentaDetalleCommandValidator : AbstractValidator<DeleteOrdenTrabajoVentaDetalleCommand>
{
    public DeleteOrdenTrabajoVentaDetalleCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Item).GreaterThan(0);
    }
}
