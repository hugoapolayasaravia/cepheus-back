using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.GetAnalisisVentasPaginated
{
    public class GetAnalisisVentasPaginatedQuery : PagedRequest, IRequest<PagedResult<AnalisisVentaResponse>>
    {
        public string? Search { get; set; }
        public string? SegmentoVentasCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
