using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.DeleteDocumentoCompraDetalle
{
    public record DeleteDocumentoCompraDetalleCommand(
        string ComprobantePagoCode, string ProveedorCode, string NumeroDocumento, int ItemNumber)
        : IRequest<DocumentoCompraResponse>;
}
