using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.CreateStockProducto
{
    public record CreateStockProductoCommand(
        string PlantaCode,
        string TipoProductoCode,
        string ProductoCode,
        decimal Cantidad
    ) : IRequest<StockProductoResponse>;
}
