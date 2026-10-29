using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.GetPrecioProducto
{
    public class GetPrecioProductoQueryHandler : IRequestHandler<GetPrecioProductoQuery, PrecioProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetPrecioProductoQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PrecioProductoResponse> Handle(GetPrecioProductoQuery request, CancellationToken cancellationToken)
        {
            var precioProducto = await _uow.Facturacion.Catalogos.PreciosProducto.Query()
                .AsNoTracking()
                .Where(p =>
                    p.FleteCode == request.FleteCode &&
                    p.ProductoTipoCode == request.ProductoTipoCode &&
                    p.ProductoCode == request.ProductoCode &&
                    p.CurrencyTypeCode == request.CurrencyTypeCode &&
                    p.CurrencyCode == request.CurrencyCode)
                .Select(p => new PrecioProductoResponse
                {
                    FleteCode = p.FleteCode,
                    ProductoTipoCode = p.ProductoTipoCode,
                    ProductoCode = p.ProductoCode,
                    CurrencyTypeCode = p.CurrencyTypeCode,
                    CurrencyCode = p.CurrencyCode,
                    Amount = p.Amount,
                    TransportAmount = p.TransportAmount,
                    FreightAmount = p.FreightAmount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    RowVersion = p.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (precioProducto is null)
            {
                throw new KeyNotFoundException("Precio de producto no encontrado para la combinación indicada.");
            }

            return precioProducto;
        }
    }
}
