using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.BloquearOrdenTrabajoVenta;

public sealed class BloquearOrdenTrabajoVentaCommandValidator : AbstractValidator<BloquearOrdenTrabajoVentaCommand>
{
    public BloquearOrdenTrabajoVentaCommandValidator() => RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
}
