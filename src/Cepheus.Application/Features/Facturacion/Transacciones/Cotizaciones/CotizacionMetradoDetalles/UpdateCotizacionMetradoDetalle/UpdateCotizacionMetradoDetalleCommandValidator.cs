using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.UpdateCotizacionMetradoDetalle
{
    public class UpdateCotizacionMetradoDetalleCommandValidator : AbstractValidator<UpdateCotizacionMetradoDetalleCommand>
    {
        public UpdateCotizacionMetradoDetalleCommandValidator()
        {
            RuleFor(x => x.NegocioCode).NotEmpty().Length(2);
            RuleFor(x => x.Year).NotEmpty().Length(4);
            RuleFor(x => x.Month).NotEmpty().Length(2);
            RuleFor(x => x.Code).NotEmpty().Length(8);
            RuleFor(x => x.LevelNumber).GreaterThan(0);
            RuleFor(x => x.Order).NotEmpty().MaximumLength(3);
            RuleFor(x => x.ProductoTipoCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");

            RuleFor(x => x.PanelCode).NotNull().MaximumLength(5);
            RuleFor(x => x.SortOrder).NotEmpty().MaximumLength(3);
            RuleFor(x => x.Times).GreaterThanOrEqualTo(0);

            RuleFor(x => x.Anchorage)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El anclaje es obligatorio.")
                .Must(v => System.Enum.TryParse<Cepheus.Domain.Facturacion.Enum.TipoAnclaje>(v, true, out _))
                .WithMessage("El anclaje debe ser uno de: Si, No, Medio.");

            RuleFor(x => x.InnerLength).GreaterThanOrEqualTo(0);
            RuleFor(x => x.OuterLength).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Support).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalMaterial).GreaterThanOrEqualTo(0);
            RuleFor(x => x.VaultCount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.WastePercentage).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Row).GreaterThanOrEqualTo(0);
            RuleFor(x => x.QuantityB).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Support2).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Width).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Area).GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaterialPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaterialIgv).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TransportPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.VaultPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.VaultTotalPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaterialPriceAlt).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TransportPriceAlt).GreaterThanOrEqualTo(0);
            RuleFor(x => x.VaultPriceAlt).GreaterThanOrEqualTo(0);
            RuleFor(x => x.VaultTotalPriceAlt).GreaterThanOrEqualTo(0);

            RuleFor(x => x.DeliveredTotalMaterial).GreaterThanOrEqualTo(0);
            RuleFor(x => x.DeliveredQuantityB).GreaterThanOrEqualTo(0);

            RuleFor(x => x.PolystyrenePrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PolystyreneTotalPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PolystyrenePriceAlt).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PolystyreneTotalPriceAlt).GreaterThanOrEqualTo(0);

            RuleFor(x => x.Widening).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SupportP).GreaterThanOrEqualTo(0);
            RuleFor(x => x.WastePercentageP).GreaterThanOrEqualTo(0);
            RuleFor(x => x.QuantityP).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Spacing).GreaterThanOrEqualTo(0);
        }
    }
}
