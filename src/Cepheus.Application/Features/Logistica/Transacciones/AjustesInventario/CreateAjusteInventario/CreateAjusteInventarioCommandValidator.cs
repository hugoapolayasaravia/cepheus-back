using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.CreateAjusteInventario;

public sealed class CreateAjusteInventarioCommandValidator : AbstractValidator<CreateAjusteInventarioCommand>
{
    public CreateAjusteInventarioCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2)
            .WithMessage("La planta es obligatoria.");

        RuleFor(x => x.FechaEntrega).NotEmpty()
            .WithMessage("La fecha de entrega es obligatoria.");

        RuleFor(x => x.Observacion).MaximumLength(1000);

        RuleForEach(x => x.Detalles).ChildRules(d =>
        {
            d.RuleFor(l => l.ArticuloCode).NotEmpty().MaximumLength(7)
                .WithMessage("El código de artículo es obligatorio.");
            d.RuleFor(l => l.Tipo).IsInEnum()
                .WithMessage("El tipo de ajuste debe ser Sobrante o Faltante.");
            d.RuleFor(l => l.Cantidad).GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a cero.");
        });
    }
}
