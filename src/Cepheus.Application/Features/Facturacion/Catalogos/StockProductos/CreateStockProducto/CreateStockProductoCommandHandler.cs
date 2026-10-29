using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.CreateStockProducto
{
    public class CreateStockProductoCommandHandler : IRequestHandler<CreateStockProductoCommand, StockProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateStockProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<StockProductoResponse> Handle(CreateStockProductoCommand request, CancellationToken cancellationToken)
        {
            var entity = new StockProducto
            {
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                TipoProductoCode = request.TipoProductoCode.Trim().ToUpperInvariant(),
                ProductoCode = request.ProductoCode.Trim().ToUpperInvariant(),
                Cantidad = request.Cantidad
            };

            await _uow.Facturacion.Catalogos.StockProductos.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static StockProductoResponse Map(StockProducto e) => new()
        {
            Id = e.Id,
            PlantaCode = e.PlantaCode,
            TipoProductoCode = e.TipoProductoCode,
            ProductoCode = e.ProductoCode,
            Cantidad = e.Cantidad,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
