using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.GetUnidadesMedidaVentaPaginated
{
    public class GetUnidadesMedidaVentaPaginatedQuery : PagedRequest, IRequest<PagedResult<UnidadMedidaVentaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
