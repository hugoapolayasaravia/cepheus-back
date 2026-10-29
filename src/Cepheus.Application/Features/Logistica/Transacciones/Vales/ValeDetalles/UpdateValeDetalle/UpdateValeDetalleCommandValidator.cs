using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.UpdateValeDetalle;

public sealed class UpdateValeDetalleCommandValidator : AbstractValidator<UpdateValeDetalleCommand>
{
    public UpdateValeDetalleCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.ValeCode).NotEmpty().MaximumLength(7);
        RuleFor(x => x.ArticuloCode).NotEmpty().MaximumLength(7);
        RuleFor(x => x.Cantidad).GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a cero.");
        RuleFor(x => x.Propiedad01).MaximumLength(7);
    }
}
