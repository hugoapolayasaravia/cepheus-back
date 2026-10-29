using FluentValidation;

namespace Cepheus.Application.Features.Comunes.ControlesVentas.UpdateControlVentas
{
    public class UpdateControlVentasCommandValidator : AbstractValidator<UpdateControlVentasCommand>
    {
        public UpdateControlVentasCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.ClosingPeriod)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El período de cierre es obligatorio.")
                .Length(6).WithMessage("El período de cierre debe tener formato YYYYMM (6 dígitos).")
                .Matches("^[0-9]{6}$").WithMessage("El período de cierre debe contener solo dígitos.");

            RuleFor(x => x.IgvPercentage).InclusiveBetween(0, 100)
                .WithMessage("El porcentaje de IGV debe estar entre 0 y 100.");

            RuleFor(x => x.WithholdingPercentage).InclusiveBetween(0, 100)
                .WithMessage("El porcentaje de retención debe estar entre 0 y 100.");

            RuleFor(x => x.DetractionPercentage).InclusiveBetween(0, 100)
                .WithMessage("El porcentaje de detracción debe estar entre 0 y 100.");

            RuleFor(x => x.IncomeTaxPercentage).InclusiveBetween(0, 100)
                .WithMessage("El porcentaje de renta debe estar entre 0 y 100.");

            RuleFor(x => x.ForeignIgvPercentage).InclusiveBetween(0, 100)
                .WithMessage("El porcentaje de IGV exterior debe estar entre 0 y 100.");

            RuleFor(x => x.WithholdingCap).GreaterThanOrEqualTo(0)
                .WithMessage("El tope de retención debe ser mayor o igual a 0.");

            RuleFor(x => x.WithholdingCapInvoice).GreaterThanOrEqualTo(0)
                .WithMessage("El tope de retención por factura debe ser mayor o igual a 0.");

            RuleFor(x => x.WorkOrderDaysLimit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.WorkOrderMaxDays).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SaleMaxDays).GreaterThanOrEqualTo(0);

            RuleFor(x => x.PurchasesProcessDate)
                .GreaterThanOrEqualTo(x => x.SalesProcessDate.AddYears(-1))
                .WithMessage("La fecha de proceso de compras parece inconsistente respecto a la de ventas.");
        }
    }
}
