using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.UpdateOrdenServicioDetalle;

public sealed class UpdateOrdenServicioDetalleCommandValidator : AbstractValidator<UpdateOrdenServicioDetalleCommand>
{
    public UpdateOrdenServicioDetalleCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.OrdenServicioCode).NotEmpty().MaximumLength(6);
        RuleFor(x => x.ItemNumber).GreaterThan(0);
        RuleFor(x => x.Detalle).NotNull().SetValidator(new OrdenServicioDetalleRequestValidator());
    }
}
