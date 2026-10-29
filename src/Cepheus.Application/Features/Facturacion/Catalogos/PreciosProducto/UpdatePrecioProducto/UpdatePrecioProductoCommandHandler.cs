using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.UpdatePrecioProducto
{
    public class UpdatePrecioProductoCommandHandler : IRequestHandler<UpdatePrecioProductoCommand, PrecioProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePrecioProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PrecioProductoResponse> Handle(UpdatePrecioProductoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.PreciosProducto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.FleteCode == request.FleteCode &&
                    p.ProductoTipoCode == request.ProductoTipoCode &&
                    p.ProductoCode == request.ProductoCode &&
                    p.CurrencyTypeCode == request.CurrencyTypeCode &&
                    p.CurrencyCode == request.CurrencyCode, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException("Precio de producto no encontrado para la combinación indicada.");
            }

            var precioProducto = new PrecioProducto
            {
                FleteCode = request.FleteCode,
                ProductoTipoCode = request.ProductoTipoCode,
                ProductoCode = request.ProductoCode,
                CurrencyTypeCode = request.CurrencyTypeCode,
                CurrencyCode = request.CurrencyCode,
                Amount = request.Amount,
                TransportAmount = request.TransportAmount,
                FreightAmount = request.FreightAmount,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.PreciosProducto.Update(precioProducto);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El precio fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new PrecioProductoResponse
            {
                FleteCode = precioProducto.FleteCode,
                ProductoTipoCode = precioProducto.ProductoTipoCode,
                ProductoCode = precioProducto.ProductoCode,
                CurrencyTypeCode = precioProducto.CurrencyTypeCode,
                CurrencyCode = precioProducto.CurrencyCode,
                Amount = precioProducto.Amount,
                TransportAmount = precioProducto.TransportAmount,
                FreightAmount = precioProducto.FreightAmount,
                CreatedAt = precioProducto.CreatedAt,
                UpdatedAt = precioProducto.UpdatedAt,
                RowVersion = precioProducto.RowVersion
            };
        }
    }
}
