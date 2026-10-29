using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.UpdateMotivoSolicitudNotaCredito
{
    public record UpdateMotivoSolicitudNotaCreditoCommand(
        string Code,
        string Name,
        string? ParentCode,
        string? MotivoDevolucionCode,
        bool AffectsStock,
        DateTime? StartDate,
        DateTime? EndDate,
        byte[] RowVersion
    ) : IRequest<MotivoSolicitudNotaCreditoResponse>;
}
