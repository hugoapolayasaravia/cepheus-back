using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.CreateUsuarioSolicitudNotaCredito
{
    public record CreateUsuarioSolicitudNotaCreditoCommand(
        string NegocioCode,
        string Stage,
        int UserId,
        bool IsPrincipal,
        bool CanEnter,
        bool CanManage,
        bool CanApprove,
        DateTime? StartDate,
        DateTime? EndDate
    ) : IRequest<UsuarioSolicitudNotaCreditoResponse>;
}
