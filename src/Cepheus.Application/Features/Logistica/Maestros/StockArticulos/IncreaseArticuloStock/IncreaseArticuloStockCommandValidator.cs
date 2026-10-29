using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.IncreaseArticuloStock
{
    public class IncreaseArticuloStockCommandValidator : AbstractValidator<IncreaseArticuloStockCommand>
    {
        public IncreaseArticuloStockCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.ArticuloCode).NotEmpty().WithMessage("El artículo es obligatorio.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad a incrementar debe ser mayor a cero.");
        }
    }
}
