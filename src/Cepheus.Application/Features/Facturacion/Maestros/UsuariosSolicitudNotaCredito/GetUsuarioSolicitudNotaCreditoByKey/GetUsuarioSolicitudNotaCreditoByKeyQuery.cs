using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.GetUsuarioSolicitudNotaCreditoByKey
{
    public record GetUsuarioSolicitudNotaCreditoByKeyQuery(string NegocioCode, string Stage, int UserId)
        : IRequest<UsuarioSolicitudNotaCreditoResponse>;
}
