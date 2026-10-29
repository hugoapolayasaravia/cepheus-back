using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.GetMotivosSolicitudNotaCreditoPaginated
{
    public class GetMotivosSolicitudNotaCreditoPaginatedQuery : PagedRequest, IRequest<PagedResult<MotivoSolicitudNotaCreditoResponse>>
    {
        public string? Search { get; set; }
        public string? ParentCode { get; set; }
        public bool? IsActive { get; set; }

        /// <summary>
        /// Si es true, solo devuelve los motivos vigentes a hoy (según FEC_INICIO / FEC_FIN).
        /// Útil para llenar el combo al registrar una solicitud.
        /// </summary>
        public bool? OnlyCurrent { get; set; }
    }
}
