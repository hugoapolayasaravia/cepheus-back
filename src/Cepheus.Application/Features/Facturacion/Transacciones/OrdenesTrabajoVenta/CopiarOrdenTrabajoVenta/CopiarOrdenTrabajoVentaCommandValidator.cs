using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.CopiarOrdenTrabajoVenta;

public sealed class CopiarOrdenTrabajoVentaCommandValidator : AbstractValidator<CopiarOrdenTrabajoVentaCommand>
{
    public CopiarOrdenTrabajoVentaCommandValidator() => RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
}
