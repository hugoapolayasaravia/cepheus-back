using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.CreateProducto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.GetProductoByCode
{
    public class GetProductoByCodeQueryHandler : IRequestHandler<GetProductoByCodeQuery, ProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetProductoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProductoResponse> Handle(GetProductoByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoProductoCode = request.TipoProductoCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var producto = await _uow.Facturacion.Maestros.Productos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TipoProductoCode == tipoProductoCode && p.Code == code, cancellationToken);

            if (producto is null)
            {
                throw new KeyNotFoundException($"Producto {tipoProductoCode}{code} no encontrado.");
            }

            return CreateProductoCommandHandler.Map(producto);
        }
    }
}
