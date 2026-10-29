using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.UpdateStockProducto
{
    public record UpdateStockProductoCommand(
        long Id,
        string PlantaCode,
        string TipoProductoCode,
        string ProductoCode,
        decimal Cantidad,
        byte[] RowVersion
    ) : IRequest<StockProductoResponse>;
}
