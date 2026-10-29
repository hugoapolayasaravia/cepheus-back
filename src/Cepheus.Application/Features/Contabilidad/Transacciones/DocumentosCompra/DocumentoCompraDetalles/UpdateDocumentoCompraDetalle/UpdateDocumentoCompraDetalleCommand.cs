using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.UpdateDocumentoCompraDetalle
{
    public record UpdateDocumentoCompraDetalleCommand(
        string ComprobantePagoCode,
        string ProveedorCode,
        string NumeroDocumento,
        int ItemNumber,
        string PlantaCode,
        string CuentaContableCode,
        string? AnexoTipoCode,
        string? AnexoCode,
        string Detalle,
        decimal Cantidad,
        string UnidadMedidaCode,
        decimal PrecioSol,
        string? NumeroGuiaVenta,
        string? ProductoTipoCode,
        string? ProductoCode
    ) : IRequest<DocumentoCompraResponse>, IDocumentoCompraLinea;
}
