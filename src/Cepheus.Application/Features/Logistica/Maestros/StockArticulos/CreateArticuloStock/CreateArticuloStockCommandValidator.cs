using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.CreateArticuloStock
{
    public class CreateArticuloStockCommandValidator : AbstractValidator<CreateArticuloStockCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateArticuloStockCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.ArticuloCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El artículo es obligatorio.")
                .MustAsync(ArticuloExists).WithMessage("El artículo indicado no existe.");

            RuleFor(x => x)
                .MustAsync(BeUniqueCombination)
                .WithMessage("Ya existe un registro de stock para este artículo en esta planta.")
                .OverridePropertyName(nameof(CreateArticuloStockCommand.ArticuloCode));

            RuleFor(x => x.InitialQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad inicial no puede ser negativa.");

            RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
            RuleFor(x => x.UnitCostUsd).GreaterThanOrEqualTo(0).WithMessage("El costo unitario en dólares no puede ser negativo.");
            RuleFor(x => x.AverageCost).GreaterThanOrEqualTo(0).WithMessage("El costo promedio no puede ser negativo.").When(x => x.AverageCost.HasValue);

            RuleFor(x => x.MinStock).GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");
            RuleFor(x => x.MaxStock).GreaterThanOrEqualTo(0).WithMessage("El stock máximo no puede ser negativo.");

            RuleFor(x => x)
                .Must(x => x.MaxStock >= x.MinStock)
                .WithMessage("El stock máximo no puede ser menor al stock mínimo.")
                .OverridePropertyName(nameof(CreateArticuloStockCommand.MaxStock));
        }

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ArticuloExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Articulos.Query().AnyAsync(a => a.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniqueCombination(CreateArticuloStockCommand command, CancellationToken ct)
            => !await _uow.Logistica.Maestros.StockArticulos.Query()
                .AnyAsync(s =>
                    s.PlantaCode == command.PlantaCode.Trim().ToUpper() &&
                    s.ArticuloCode == command.ArticuloCode.Trim().ToUpper(), ct);
    }
}
