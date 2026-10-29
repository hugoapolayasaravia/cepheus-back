using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetDetraccionItemDocumentos
{
    /// <summary>Documentos de un ítem del lote (legacy: SP com_lista_det_doc_prv).</summary>
    public record GetDetraccionItemDocumentosQuery(
        string Year, string Period, string LotNumber, string ProveedorCode, int ItemNumber)
        : IRequest<List<DetraccionDocumentoResponse>>;
}
