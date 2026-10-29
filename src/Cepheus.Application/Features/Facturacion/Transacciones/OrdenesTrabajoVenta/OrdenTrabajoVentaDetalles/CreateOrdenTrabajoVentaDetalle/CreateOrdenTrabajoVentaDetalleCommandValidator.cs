using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.CreateOrdenTrabajoVentaDetalle;

public sealed class CreateOrdenTrabajoVentaDetalleCommandValidator : AbstractValidator<CreateOrdenTrabajoVentaDetalleCommand>
{
    public CreateOrdenTrabajoVentaDetalleCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull().SetValidator(new OrdenTrabajoVentaDetalleRequestValidator());
    }
}
