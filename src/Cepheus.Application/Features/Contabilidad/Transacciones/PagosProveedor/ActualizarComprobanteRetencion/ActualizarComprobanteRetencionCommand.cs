using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.ActualizarComprobanteRetencion
{
    /// <summary>Legacy w_comprobante: corrige el número del comprobante de retención del pago.</summary>
    public record ActualizarComprobanteRetencionCommand(
        string Code,
        string NumeroComprobanteRetencion,
        DateTime? FechaComprobanteRetencion
    ) : IRequest<PagoProveedorResponse>;
}
