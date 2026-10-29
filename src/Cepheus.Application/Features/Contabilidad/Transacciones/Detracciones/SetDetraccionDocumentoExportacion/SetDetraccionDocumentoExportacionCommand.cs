using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.SetDetraccionDocumentoExportacion
{
    /// <summary>
    /// Marca o desmarca un documento del lote para el depósito masivo y recalcula el depósito del ítem
    /// y el total del lote (legacy: columna FLG_EXPORTA del detalle de proveedor).
    /// </summary>
    public record SetDetraccionDocumentoExportacionCommand(
        string Year,
        string Period,
        string LotNumber,
        string ProveedorCode,
        int ItemNumber,
        string ComprobantePagoCode,
        string NumeroDocumento,
        bool IncludeInExport
    ) : IRequest<DetraccionResponse>;
}
