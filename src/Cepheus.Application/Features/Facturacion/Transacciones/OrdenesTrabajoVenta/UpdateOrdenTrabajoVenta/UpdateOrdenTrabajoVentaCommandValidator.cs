using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.UpdateOrdenTrabajoVenta;

public sealed class UpdateOrdenTrabajoVentaCommandValidator : AbstractValidator<UpdateOrdenTrabajoVentaCommand>
{
    private sealed class DataValidator : OrdenTrabajoVentaFieldsValidator<OrdenTrabajoVentaUpdateFields>
    {
    }

    public UpdateOrdenTrabajoVentaCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull().SetValidator(new DataValidator());
    }
}
