using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.RemoveDocumentoCancelacionVenta
{
    /// <summary>Retira un documento de una cancelación Pendiente y sin asiento (legacy ue_eli_lin_det) y le devuelve su saldo.</summary>
    public record RemoveDocumentoCancelacionVentaCommand(string Code, string Item, byte[] RowVersion)
        : IRequest<CancelacionVentaResponse>;
}
