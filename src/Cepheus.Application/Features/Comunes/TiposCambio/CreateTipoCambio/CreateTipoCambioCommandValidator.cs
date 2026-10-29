using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposCambio.CreateTipoCambio
{
    public class CreateTipoCambioCommandValidator : AbstractValidator<CreateTipoCambioCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoCambioCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MaximumLength(3).WithMessage("La moneda no puede exceder los 3 caracteres.")
                .Must(m => !TipoCambioRules.EsMonedaBase(m))
                    .WithMessage($"La moneda base ({TipoCambioRules.MonedaBase}) no tiene tipo de cambio.")
                .MustAsync(MonedaExists).WithMessage("La moneda indicada no existe o no está activa.");

            RuleFor(x => x.Date)
                .NotEqual(default(DateOnly)).WithMessage("La fecha es obligatoria.");

            RuleFor(x => x)
                .MustAsync(BeUnique)
                .WithMessage("Ya existe un tipo de cambio registrado para esa moneda y fecha.")
                .OverridePropertyName("Date")
                .When(x => !string.IsNullOrWhiteSpace(x.MonedaCode) && x.Date != default);

            RuleFor(x => x.SellRate)
                .GreaterThan(0).WithMessage("El tipo de cambio venta debe ser mayor a 0.");

            RuleFor(x => x.BuyRate)
                .GreaterThan(0).WithMessage("El tipo de cambio compra debe ser mayor a 0.");

            RuleFor(x => x)
                .Must(x => x.BuyRate <= x.SellRate)
                .WithMessage("El tipo de cambio compra no puede ser mayor al de venta.")
                .OverridePropertyName("BuyRate");
        }

        private async Task<bool> MonedaExists(string code, CancellationToken cancellationToken)
        {
            var moneda = TipoCambioRules.Normalizar(code);
            return await _uow.Comunes.Monedas.Query()
                .AnyAsync(m => m.Code == moneda && m.IsActive, cancellationToken);
        }

        private async Task<bool> BeUnique(CreateTipoCambioCommand command, CancellationToken cancellationToken)
        {
            var moneda = TipoCambioRules.Normalizar(command.MonedaCode);
            return !await _uow.Comunes.TiposCambio.Query()
                .AnyAsync(t => t.MonedaCode == moneda && t.Date == command.Date, cancellationToken);
        }
    }
}
