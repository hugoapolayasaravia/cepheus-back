using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.CreateOrdenServicioDetalle;

public sealed class CreateOrdenServicioDetalleCommandValidator : AbstractValidator<CreateOrdenServicioDetalleCommand>
{
    public CreateOrdenServicioDetalleCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.OrdenServicioCode).NotEmpty().MaximumLength(6);
        RuleFor(x => x.Detalle).NotNull().SetValidator(new OrdenServicioDetalleRequestValidator());
    }
}
