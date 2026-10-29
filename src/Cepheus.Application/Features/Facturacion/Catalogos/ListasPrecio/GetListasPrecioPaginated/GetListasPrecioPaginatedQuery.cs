using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.GetListasPrecioPaginated
{
    public class GetListasPrecioPaginatedQuery : PagedRequest, IRequest<PagedResult<ListaPrecioResponse>>
    {
        public string? TipoProductoCode { get; set; }
        public string? ProductoCode { get; set; }
        public bool? IsActive { get; set; }
        public bool? Vigente { get; set; }
    }
}
