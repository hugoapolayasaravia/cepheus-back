using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposCambio.GetTiposCambioPaginated
{
    public class GetTiposCambioPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoCambioResponse>>
    {
        /// <summary>Filtra por moneda (ej. USD). Sin valor trae todas.</summary>
        public string? MonedaCode { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
