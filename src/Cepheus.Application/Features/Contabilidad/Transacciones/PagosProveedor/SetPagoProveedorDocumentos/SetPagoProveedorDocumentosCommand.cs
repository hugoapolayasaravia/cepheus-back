using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.SetPagoProveedorDocumentos
{
    /// <summary>
    /// Documentos que se cancelan con el pago. Reemplaza el detalle completo.
    /// PagoSol es opcional: si no se envía se usa el pago sugerido (legacy ue_control).
    /// En notas de crédito el pago va en negativo.
    /// </summary>
    public record SetPagoProveedorDocumentosCommand(
        string Code,
        List<PagoProveedorDocumentoInput> Documentos
    ) : IRequest<PagoProveedorResponse>;

    public record PagoProveedorDocumentoInput(
        string ComprobantePagoCode,
        string NumeroDocumento,
        decimal? PagoSol
    );
}
