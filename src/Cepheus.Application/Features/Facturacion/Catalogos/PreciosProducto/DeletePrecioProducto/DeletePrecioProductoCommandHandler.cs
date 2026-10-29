using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.DeletePrecioProducto
{
    public class DeletePrecioProductoCommandHandler : IRequestHandler<DeletePrecioProductoCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeletePrecioProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeletePrecioProductoCommand request, CancellationToken cancellationToken)
        {
            var precioProducto = await _uow.Facturacion.Catalogos.PreciosProducto.Query()
                .FirstOrDefaultAsync(p =>
                    p.FleteCode == request.FleteCode &&
                    p.ProductoTipoCode == request.ProductoTipoCode &&
                    p.ProductoCode == request.ProductoCode &&
                    p.CurrencyTypeCode == request.CurrencyTypeCode &&
                    p.CurrencyCode == request.CurrencyCode, cancellationToken);

            if (precioProducto is null)
            {
                throw new KeyNotFoundException("Precio de producto no encontrado para la combinación indicada.");
            }

            _uow.Facturacion.Catalogos.PreciosProducto.Remove(precioProducto);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
