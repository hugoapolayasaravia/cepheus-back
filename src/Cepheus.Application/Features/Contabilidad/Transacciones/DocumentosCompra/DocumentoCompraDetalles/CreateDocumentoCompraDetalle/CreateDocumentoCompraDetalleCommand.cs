using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.CreateDocumentoCompraDetalle
{
    public record CreateDocumentoCompraDetalleCommand(
        string ComprobantePagoCode,
        string ProveedorCode,
        string NumeroDocumento,
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
