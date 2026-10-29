using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.UpdateStockProducto
{
    public class UpdateStockProductoCommandHandler : IRequestHandler<UpdateStockProductoCommand, StockProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateStockProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<StockProductoResponse> Handle(UpdateStockProductoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.StockProductos.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Stock {request.Id} no encontrado.");
            }

            var entity = new StockProducto
            {
                Id = request.Id,
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                TipoProductoCode = request.TipoProductoCode.Trim().ToUpperInvariant(),
                ProductoCode = request.ProductoCode.Trim().ToUpperInvariant(),
                Cantidad = request.Cantidad,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.StockProductos.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El stock fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateStockProducto.CreateStockProductoCommandHandler.Map(entity);
        }
    }
}
