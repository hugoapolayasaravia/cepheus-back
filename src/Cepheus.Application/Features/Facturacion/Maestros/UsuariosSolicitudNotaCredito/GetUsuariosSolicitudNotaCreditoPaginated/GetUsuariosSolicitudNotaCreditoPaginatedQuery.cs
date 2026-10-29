using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.GetUsuariosSolicitudNotaCreditoPaginated
{
    public class GetUsuariosSolicitudNotaCreditoPaginatedQuery : PagedRequest, IRequest<PagedResult<UsuarioSolicitudNotaCreditoResponse>>
    {
        public string? NegocioCode { get; set; }
        public string? Stage { get; set; }
        public int? UserId { get; set; }
        public bool? IsPrincipal { get; set; }
        public bool? IsActive { get; set; }
        public string? Search { get; set; }
    }
}
