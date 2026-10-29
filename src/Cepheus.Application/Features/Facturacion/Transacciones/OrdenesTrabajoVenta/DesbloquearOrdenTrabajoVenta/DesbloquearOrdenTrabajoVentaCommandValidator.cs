using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.DesbloquearOrdenTrabajoVenta;

public sealed class DesbloquearOrdenTrabajoVentaCommandValidator : AbstractValidator<DesbloquearOrdenTrabajoVentaCommand>
{
    public DesbloquearOrdenTrabajoVentaCommandValidator() => RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
}
