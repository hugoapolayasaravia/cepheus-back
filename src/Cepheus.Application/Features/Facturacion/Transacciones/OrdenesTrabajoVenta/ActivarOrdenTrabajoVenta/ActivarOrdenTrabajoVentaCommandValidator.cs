using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.ActivarOrdenTrabajoVenta;

public sealed class ActivarOrdenTrabajoVentaCommandValidator : AbstractValidator<ActivarOrdenTrabajoVentaCommand>
{
    public ActivarOrdenTrabajoVentaCommandValidator() => RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
}
