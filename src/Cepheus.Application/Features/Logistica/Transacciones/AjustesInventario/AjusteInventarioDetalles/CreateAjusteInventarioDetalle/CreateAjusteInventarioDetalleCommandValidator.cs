using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AjusteInventarioDetalles.CreateAjusteInventarioDetalle;

public sealed class CreateAjusteInventarioDetalleCommandValidator
    : AbstractValidator<CreateAjusteInventarioDetalleCommand>
{
    public CreateAjusteInventarioDetalleCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.AjusteCode).NotEmpty().MaximumLength(6);
        RuleFor(x => x.ArticuloCode).NotEmpty().MaximumLength(7)
            .WithMessage("El código de artículo es obligatorio.");
        RuleFor(x => x.Tipo).IsInEnum()
            .WithMessage("El tipo de ajuste debe ser Sobrante o Faltante.");
        RuleFor(x => x.Cantidad).GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a cero.");
    }
}
