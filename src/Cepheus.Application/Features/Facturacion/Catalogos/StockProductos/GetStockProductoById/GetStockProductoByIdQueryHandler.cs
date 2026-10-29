using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.CreateStockProducto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.GetStockProductoById
{
    public class GetStockProductoByIdQueryHandler : IRequestHandler<GetStockProductoByIdQuery, StockProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetStockProductoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<StockProductoResponse> Handle(GetStockProductoByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.StockProductos.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Stock {request.Id} no encontrado.");
            }

            return CreateStockProductoCommandHandler.Map(entity);
        }
    }
}
