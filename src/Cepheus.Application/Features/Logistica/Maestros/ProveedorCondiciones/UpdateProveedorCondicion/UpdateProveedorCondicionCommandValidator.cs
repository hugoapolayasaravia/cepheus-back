using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.UpdateProveedorCondicion
{
    public class UpdateProveedorCondicionCommandValidator : AbstractValidator<UpdateProveedorCondicionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProveedorCondicionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.FormaPagoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La forma de pago es obligatoria.")
                .MustAsync(FormaPagoExists).WithMessage("La forma de pago indicada no existe.");

            RuleFor(x => x.PaymentTermDays)
                .GreaterThanOrEqualTo(0).WithMessage("El plazo de pago no puede ser negativo.");

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MustAsync(MonedaExists).WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.CreditLimit)
                .GreaterThanOrEqualTo(0).WithMessage("El límite de crédito no puede ser negativo.")
                .When(x => x.CreditLimit.HasValue);

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("El descuento debe estar entre 0 y 100.")
                .When(x => x.DiscountPercentage.HasValue);
        }

        private async Task<bool> FormaPagoExists(string formaPagoCode, CancellationToken cancellationToken)
            => await _uow.Logistica.Catalogos.FormasPago.Query()
                .AnyAsync(f => f.Code == formaPagoCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> MonedaExists(string monedaCode, CancellationToken cancellationToken)
            => await _uow.Comunes.Monedas.Query()
                .AnyAsync(m => m.Code == monedaCode.Trim().ToUpper(), cancellationToken);
    }
}
