using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.UpdateArticuloStockThresholds
{
    public class UpdateArticuloStockThresholdsCommandValidator : AbstractValidator<UpdateArticuloStockThresholdsCommand>
    {
        public UpdateArticuloStockThresholdsCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.ArticuloCode).NotEmpty().WithMessage("El artículo es obligatorio.");

            RuleFor(x => x.MinStock).GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");
            RuleFor(x => x.MaxStock).GreaterThanOrEqualTo(0).WithMessage("El stock máximo no puede ser negativo.");

            RuleFor(x => x)
                .Must(x => x.MaxStock >= x.MinStock)
                .WithMessage("El stock máximo no puede ser menor al stock mínimo.")
                .OverridePropertyName(nameof(UpdateArticuloStockThresholdsCommand.MaxStock));

            RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
            RuleFor(x => x.UnitCostUsd).GreaterThanOrEqualTo(0).WithMessage("El costo unitario en dólares no puede ser negativo.");
            RuleFor(x => x.AverageCost).GreaterThanOrEqualTo(0).WithMessage("El costo promedio no puede ser negativo.").When(x => x.AverageCost.HasValue);

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
