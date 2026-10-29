using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.GetStockProductoById
{
    public record GetStockProductoByIdQuery(long Id) : IRequest<StockProductoResponse>;
}
