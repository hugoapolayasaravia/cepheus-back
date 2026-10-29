using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.GetCotizacionMetradoDetallesByResumen
{
    public class GetCotizacionMetradoDetallesByResumenQueryHandler
        : IRequestHandler<GetCotizacionMetradoDetallesByResumenQuery, List<CotizacionMetradoDetalleResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCotizacionMetradoDetallesByResumenQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<CotizacionMetradoDetalleResponse>> Handle(
            GetCotizacionMetradoDetallesByResumenQuery request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var detalles = await _uow.Facturacion.Transacciones.CotizacionesMetradoDetalle.Query()
                .AsNoTracking()
                .Where(d => d.NegocioCode == negocio && d.Year == request.Year && d.Month == request.Month
                         && d.Code == code && d.LevelNumber == request.LevelNumber)
                .OrderBy(d => d.SortOrder)
                .ToListAsync(cancellationToken);

            return detalles.Select(d => new CotizacionMetradoDetalleResponse
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
            }).ToList();
        }
    }
}
