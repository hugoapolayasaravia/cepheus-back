using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.ToggleUsuarioSolicitudNotaCreditoStatus
{
    public record ToggleUsuarioSolicitudNotaCreditoStatusCommand(string NegocioCode, string Stage, int UserId) : IRequest<bool>;
}
