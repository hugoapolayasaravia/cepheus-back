using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.Common;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.CreateCotizacionMetradoDetalle
{
    public class CreateCotizacionMetradoDetalleCommandHandler
        : IRequestHandler<CreateCotizacionMetradoDetalleCommand, CotizacionMetradoDetalleResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionMetradoDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionMetradoDetalleResponse> Handle(CreateCotizacionMetradoDetalleCommand request, CancellationToken cancellationToken)
        {
            var detalle = new CotizacionMetradoDetalle
            {
                NegocioCode = request.NegocioCode.Trim().ToUpperInvariant(),
                Year = request.Year,
                Month = request.Month,
                Code = request.Code.Trim().ToUpperInvariant(),
                LevelNumber = request.LevelNumber,
                Order = request.Order.Trim(),

                ProductoTipoCode = request.ProductoTipoCode.Trim().ToUpperInvariant(),
                ProductoCode = request.ProductoCode.Trim().ToUpperInvariant(),

                PanelCode = request.PanelCode.Trim(),
                Times = request.Times,
                InnerLength = request.InnerLength,
                OuterLength = request.OuterLength,
                Support = request.Support,
                Quantity = request.Quantity,
                TotalMaterial = request.TotalMaterial,
                VaultCount = request.VaultCount,
                WastePercentage = request.WastePercentage,
                Row = request.Row,
                QuantityB = request.QuantityB,
                Support2 = request.Support2,
                Width = request.Width,
                Area = request.Area,

                MaterialPrice = request.MaterialPrice,
                MaterialIgv = request.MaterialIgv,
                TransportPrice = request.TransportPrice,
                VaultPrice = request.VaultPrice,
                VaultTotalPrice = request.VaultTotalPrice,

                MaterialPriceAlt = request.MaterialPriceAlt,
                TransportPriceAlt = request.TransportPriceAlt,
                VaultPriceAlt = request.VaultPriceAlt,
                VaultTotalPriceAlt = request.VaultTotalPriceAlt,

                SortOrder = request.SortOrder.Trim(),

                DeliveredTotalMaterial = request.DeliveredTotalMaterial,
                DeliveredQuantityB = request.DeliveredQuantityB,

                PolystyrenePrice = request.PolystyrenePrice,
                PolystyreneTotalPrice = request.PolystyreneTotalPrice,
                PolystyrenePriceAlt = request.PolystyrenePriceAlt,
                PolystyreneTotalPriceAlt = request.PolystyreneTotalPriceAlt,

                Widening = request.Widening,
                SupportP = request.SupportP,
                WastePercentageP = request.WastePercentageP,
                QuantityP = request.QuantityP,

                Anchorage = System.Enum.Parse<Cepheus.Domain.Facturacion.Enum.TipoAnclaje>(request.Anchorage, true),
                Spacing = request.Spacing
            };

            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .AsNoTracking()
                .FirstAsync(c => c.NegocioCode == detalle.NegocioCode && c.Year == detalle.Year
                              && c.Month == detalle.Month && c.Code == detalle.Code, cancellationToken);

            CotizacionMetradoDetalleCalculator.Apply(detalle, cotizacion.MetradoCalculationSystem);

            await _uow.Facturacion.Transacciones.CotizacionesMetradoDetalle.AddAsync(detalle, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await CotizacionMetradoResumenRecalculator.RecalculateAsync(
                _uow, detalle.NegocioCode, detalle.Year, detalle.Month, detalle.Code, detalle.LevelNumber, cancellationToken);

            return Map(detalle);
        }

        internal static CotizacionMetradoDetalleResponse Map(CotizacionMetradoDetalle d) => new()
        {
            NegocioCode = d.NegocioCode,
            Year = d.Year,
            Month = d.Month,
            Code = d.Code,
            LevelNumber = d.LevelNumber,
            Order = d.Order,
            ProductoTipoCode = d.ProductoTipoCode,
            ProductoCode = d.ProductoCode,
            PanelCode = d.PanelCode,
            Times = d.Times,
            InnerLength = d.InnerLength,
            OuterLength = d.OuterLength,
            Support = d.Support,
            Quantity = d.Quantity,
            TotalMaterial = d.TotalMaterial,
            VaultCount = d.VaultCount,
            WastePercentage = d.WastePercentage,
            Row = d.Row,
            QuantityB = d.QuantityB,
            Support2 = d.Support2,
            Width = d.Width,
            Area = d.Area,
            MaterialPrice = d.MaterialPrice,
            MaterialIgv = d.MaterialIgv,
            TransportPrice = d.TransportPrice,
            VaultPrice = d.VaultPrice,
            VaultTotalPrice = d.VaultTotalPrice,
            MaterialPriceAlt = d.MaterialPriceAlt,
            TransportPriceAlt = d.TransportPriceAlt,
            VaultPriceAlt = d.VaultPriceAlt,
            VaultTotalPriceAlt = d.VaultTotalPriceAlt,
            SortOrder = d.SortOrder,
            DeliveredTotalMaterial = d.DeliveredTotalMaterial,
            DeliveredQuantityB = d.DeliveredQuantityB,
            PolystyrenePrice = d.PolystyrenePrice,
            PolystyreneTotalPrice = d.PolystyreneTotalPrice,
            PolystyrenePriceAlt = d.PolystyrenePriceAlt,
            PolystyreneTotalPriceAlt = d.PolystyreneTotalPriceAlt,
            Widening = d.Widening,
            SupportP = d.SupportP,
            WastePercentageP = d.WastePercentageP,
            QuantityP = d.QuantityP,
            Anchorage = d.Anchorage.ToString(),
            Spacing = d.Spacing,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            RowVersion = d.RowVersion
        };
    }
}
