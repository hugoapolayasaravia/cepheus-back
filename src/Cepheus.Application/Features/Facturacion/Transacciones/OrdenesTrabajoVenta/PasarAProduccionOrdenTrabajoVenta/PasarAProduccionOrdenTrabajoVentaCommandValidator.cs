using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.PasarAProduccionOrdenTrabajoVenta;

public sealed class PasarAProduccionOrdenTrabajoVentaCommandValidator : AbstractValidator<PasarAProduccionOrdenTrabajoVentaCommand>
{
    public PasarAProduccionOrdenTrabajoVentaCommandValidator() => RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
}
