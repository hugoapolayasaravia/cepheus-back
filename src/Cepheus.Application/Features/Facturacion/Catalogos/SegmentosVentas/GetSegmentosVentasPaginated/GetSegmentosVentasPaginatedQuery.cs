using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.GetSegmentosVentasPaginated
{
    public class GetSegmentosVentasPaginatedQuery : PagedRequest, IRequest<PagedResult<SegmentoVentasResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
