using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.CreateDocumentoCompra
{
    /// <summary>Origen: OrigenDocumentoCompra (Contabilidad, CajaChica, Logistica, Transportes). Procedencia: OrdenServicio, OrdenCompra, CompraDirecta.</summary>
    public record CreateDocumentoCompraCommand(
        string ComprobantePagoCode,
        string NumeroDocumento,
        string ProveedorCode,
        DateTime FechaEmision,
        DateTime FechaRecepcion,
        DateTime FechaVencimiento,
        string FormaPagoCode,
        string MonedaCode,
        DateOnly? TipoCambioFecha,
        decimal NoGravableSol,
        decimal ServicioSol,
        decimal IgvExteriorSol,
        string Glosa,
        string Origen,
        string? Procedencia,
        string? TipoBienCode,
        string? TipoOperacionCode,
        string? OperacionDetraccion,
        string? ConstanciaDetraccion,
        DateTime? FechaConstanciaDetraccion,
        string? NumeroDetraccion,
        DateTime? FechaEmisionDetraccion,
        decimal MontoDetraccionSol,
        string? DocumentoOrigenComprobantePagoCode,
        string? DocumentoOrigenNumero,
        string? OrdenCompraAdministracionCode,
        List<CreateDocumentoCompraDetalleInput> Detalles,
        DateTime? FechaDocumento = null
    ) : IRequest<DocumentoCompraResponse>, IDocumentoCompraCabecera;

    public record CreateDocumentoCompraDetalleInput(
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
    );
}
