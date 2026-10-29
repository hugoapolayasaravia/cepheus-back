using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.Common;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.CreateCotizacionDetalle
{
    public class CreateCotizacionDetalleCommandHandler
        : IRequestHandler<CreateCotizacionDetalleCommand, CotizacionDetalleResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionDetalleResponse> Handle(CreateCotizacionDetalleCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var detalle = new CotizacionDetalle
            {
                NegocioCode = negocio,
                Year = request.Year,
                Month = request.Month,
                Code = code,
                ProductoTipoCode = request.ProductoTipoCode.Trim().ToUpperInvariant(),
                ProductoCode = request.ProductoCode.Trim().ToUpperInvariant(),
                UnitCode = request.UnitCode.Trim().ToUpperInvariant(),
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                Observations = request.Observations.Trim(),
                Order = request.Order,
                DeliveredQuantity = 0
            };

            await _uow.Facturacion.Transacciones.CotizacionesDetalle.AddAsync(detalle, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await CotizacionTotalesRecalculator.RecalculateAsync(_uow, negocio, request.Year, request.Month, code, cancellationToken);

            return Map(detalle);
        }

        internal static CotizacionDetalleResponse Map(CotizacionDetalle d) => new()
        {
            NegocioCode = d.NegocioCode,
            Year = d.Year,
            Month = d.Month,
            Code = d.Code,
            Item = d.Item,
            ProductoTipoCode = d.ProductoTipoCode,
            ProductoCode = d.ProductoCode,
            UnitCode = d.UnitCode,
            Quantity = d.Quantity,
            UnitPrice = d.UnitPrice,
            Total = d.Total,
            Observations = d.Observations,
            Order = d.Order,
            DeliveredQuantity = d.DeliveredQuantity,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            RowVersion = d.RowVersion
        };
    }
}
