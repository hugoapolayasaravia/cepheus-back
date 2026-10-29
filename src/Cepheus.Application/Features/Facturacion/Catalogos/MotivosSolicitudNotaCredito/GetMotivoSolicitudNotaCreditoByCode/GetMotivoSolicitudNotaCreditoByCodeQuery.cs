using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.GetMotivoSolicitudNotaCreditoByCode
{
    public record GetMotivoSolicitudNotaCreditoByCodeQuery(string Code) : IRequest<MotivoSolicitudNotaCreditoResponse>;
}
