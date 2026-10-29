using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.ToggleMotivoSolicitudNotaCreditoStatus
{
    public record ToggleMotivoSolicitudNotaCreditoStatusCommand(string Code) : IRequest<bool>;
}
