using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.GetStockProductosPaginated
{
    public class GetStockProductosPaginatedQuery : PagedRequest, IRequest<PagedResult<StockProductoResponse>>
    {
        public string? PlantaCode { get; set; }
        public string? TipoProductoCode { get; set; }
        public string? ProductoCode { get; set; }
    }
}
