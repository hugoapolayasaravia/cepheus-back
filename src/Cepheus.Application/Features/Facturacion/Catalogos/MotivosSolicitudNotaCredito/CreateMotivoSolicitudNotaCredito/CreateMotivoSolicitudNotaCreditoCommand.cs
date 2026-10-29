using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.CreateMotivoSolicitudNotaCredito
{
    public record CreateMotivoSolicitudNotaCreditoCommand(
        string Name,
        string? ParentCode,
        string? MotivoDevolucionCode,
        bool AffectsStock,
        DateTime? StartDate,
        DateTime? EndDate
    ) : IRequest<MotivoSolicitudNotaCreditoResponse>;
}
