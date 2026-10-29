using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.DecreaseArticuloStock
{
    public class DecreaseArticuloStockCommandValidator : AbstractValidator<DecreaseArticuloStockCommand>
    {
        public DecreaseArticuloStockCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.ArticuloCode).NotEmpty().WithMessage("El artículo es obligatorio.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad a reducir debe ser mayor a cero.");
        }
    }
}
