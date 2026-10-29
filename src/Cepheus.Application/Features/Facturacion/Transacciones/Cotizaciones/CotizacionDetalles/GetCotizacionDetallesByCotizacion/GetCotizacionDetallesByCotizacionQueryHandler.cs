using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.GetCotizacionDetallesByCotizacion
{
    public class GetCotizacionDetallesByCotizacionQueryHandler
        : IRequestHandler<GetCotizacionDetallesByCotizacionQuery, List<CotizacionDetalleResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCotizacionDetallesByCotizacionQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<CotizacionDetalleResponse>> Handle(
            GetCotizacionDetallesByCotizacionQuery request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var detalles = await _uow.Facturacion.Transacciones.CotizacionesDetalle.Query()
                .AsNoTracking()
                .Where(d => d.NegocioCode == negocio && d.Year == request.Year
                         && d.Month == request.Month && d.Code == code)
                .OrderBy(d => d.Order)
                .ToListAsync(cancellationToken);

            return detalles.Select(d => new CotizacionDetalleResponse
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
            }).ToList();
        }
    }
}
