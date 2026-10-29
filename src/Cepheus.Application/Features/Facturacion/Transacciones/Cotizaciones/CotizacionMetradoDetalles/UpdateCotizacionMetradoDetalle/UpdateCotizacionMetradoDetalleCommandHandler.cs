using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.CreateCotizacionMetradoDetalle;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.UpdateCotizacionMetradoDetalle
{
    public class UpdateCotizacionMetradoDetalleCommandHandler
        : IRequestHandler<UpdateCotizacionMetradoDetalleCommand, CotizacionMetradoDetalleResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCotizacionMetradoDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionMetradoDetalleResponse> Handle(UpdateCotizacionMetradoDetalleCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();
            var tipoProducto = request.ProductoTipoCode.Trim().ToUpperInvariant();
            var producto = request.ProductoCode.Trim().ToUpperInvariant();
            var orden = request.Order.Trim();

            var current = await _uow.Facturacion.Transacciones.CotizacionesMetradoDetalle.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.NegocioCode == negocio && d.Year == request.Year && d.Month == request.Month
                                       && d.Code == code && d.LevelNumber == request.LevelNumber && d.Order == orden
                                       && d.ProductoTipoCode == tipoProducto && d.ProductoCode == producto,
                                     cancellationToken);

            if (current is null)
                throw new KeyNotFoundException(
                    $"Línea {orden}/{tipoProducto}{producto} del nivel {request.LevelNumber} de la cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            var detalle = new CotizacionMetradoDetalle
            {
                NegocioCode = negocio,
                Year = request.Year,
                Month = request.Month,
                Code = code,
                LevelNumber = request.LevelNumber,
                Order = orden,
                ProductoTipoCode = tipoProducto,
                ProductoCode = producto,

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
                Spacing = request.Spacing,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .AsNoTracking()
                .FirstAsync(c => c.NegocioCode == negocio && c.Year == request.Year
                              && c.Month == request.Month && c.Code == code, cancellationToken);

            CotizacionMetradoDetalleCalculator.Apply(detalle, cotizacion.MetradoCalculationSystem);

            _uow.Facturacion.Transacciones.CotizacionesMetradoDetalle.Update(detalle);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La línea de metrado fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            await CotizacionMetradoResumenRecalculator.RecalculateAsync(
                _uow, negocio, request.Year, request.Month, code, request.LevelNumber, cancellationToken);

            return CreateCotizacionMetradoDetalleCommandHandler.Map(detalle);
        }
    }
}
