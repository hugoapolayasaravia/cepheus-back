using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.UpdateDetraccionDocumentoConstancia
{
    /// <summary>
    /// Registra a mano la operación, constancia y fecha de detracción de un documento que NO se exporta
    /// (legacy: edición de operacion_det, constancia_det y fecconstancia_det en el detalle de proveedor).
    /// </summary>
    public record UpdateDetraccionDocumentoConstanciaCommand(
        string Year,
        string Period,
        string LotNumber,
        string ProveedorCode,
        int ItemNumber,
        string ComprobantePagoCode,
        string NumeroDocumento,
        string? OperacionDetraccion,
        string? ConstanciaDetraccion,
        DateTime? FechaConstanciaDetraccion
    ) : IRequest<DetraccionDocumentoResponse>;
}
