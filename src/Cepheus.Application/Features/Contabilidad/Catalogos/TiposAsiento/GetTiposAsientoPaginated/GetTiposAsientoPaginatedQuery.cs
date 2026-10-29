using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.GetTiposAsientoPaginated
{
    public class GetTiposAsientoPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoAsientoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public TipoCambioAplicable? ExchangeRateType { get; set; }
        public ValorizacionAsiento? Valuation { get; set; }
    }
}
