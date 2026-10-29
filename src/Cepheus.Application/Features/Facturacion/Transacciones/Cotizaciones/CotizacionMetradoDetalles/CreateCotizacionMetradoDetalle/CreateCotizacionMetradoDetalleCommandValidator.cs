using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.CreateCotizacionMetradoDetalle
{
    public class CreateCotizacionMetradoDetalleCommandValidator : AbstractValidator<CreateCotizacionMetradoDetalleCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionMetradoDetalleCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode).NotEmpty().Length(2);
            RuleFor(x => x.Year).NotEmpty().Length(4);
            RuleFor(x => x.Month).NotEmpty().Length(2);
            RuleFor(x => x.Code).NotEmpty().Length(8);
            RuleFor(x => x.LevelNumber).GreaterThan(0);
            RuleFor(x => x.Order).NotEmpty().MaximumLength(3);

            RuleFor(x => x)
                .MustAsync(NivelEditable)
                .WithMessage("Solo se pueden agregar líneas de metrado a un nivel de una cotización Pendiente y modificable.");

            RuleFor(x => x.ProductoTipoCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);

            RuleFor(x => x)
                .MustAsync(ProductoExists)
                .WithMessage("El producto indicado no existe.");

            RuleFor(x => x)
                .MustAsync(LineaUnica)
                .WithMessage("Ya existe una línea con ese orden/producto en este nivel.");

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

        private async Task<bool> NivelEditable(CreateCotizacionMetradoDetalleCommand c, CancellationToken ct)
        {
            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(x => x.NegocioCode == c.NegocioCode.Trim().ToUpper() && x.Year == c.Year
                                        && x.Month == c.Month && x.Code == c.Code.Trim().ToUpper(), ct);

            if (cotizacion is null || !cotizacion.IsEditable || cotizacion.Status != EstadoCotizacion.Pendiente)
                return false;

            return await _uow.Facturacion.Transacciones.CotizacionesMetradoResumen.Query()
                .AnyAsync(r => r.NegocioCode == c.NegocioCode.Trim().ToUpper() && r.Year == c.Year && r.Month == c.Month
                            && r.Code == c.Code.Trim().ToUpper() && r.LevelNumber == c.LevelNumber, ct);
        }

        private async Task<bool> ProductoExists(CreateCotizacionMetradoDetalleCommand c, CancellationToken ct)
            => await _uow.Facturacion.Maestros.Productos.Query()
                .AnyAsync(p => p.TipoProductoCode == c.ProductoTipoCode.Trim().ToUpper()
                             && p.Code == c.ProductoCode.Trim().ToUpper(), ct);

        private async Task<bool> LineaUnica(CreateCotizacionMetradoDetalleCommand c, CancellationToken ct)
            => !await _uow.Facturacion.Transacciones.CotizacionesMetradoDetalle.Query()
                .AnyAsync(d => d.NegocioCode == c.NegocioCode.Trim().ToUpper() && d.Year == c.Year && d.Month == c.Month
                            && d.Code == c.Code.Trim().ToUpper() && d.LevelNumber == c.LevelNumber
                            && d.Order == c.Order.Trim()
                            && d.ProductoTipoCode == c.ProductoTipoCode.Trim().ToUpper()
                            && d.ProductoCode == c.ProductoCode.Trim().ToUpper(), ct);
    }
}
