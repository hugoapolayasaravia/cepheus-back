namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.Common
{
    /// <summary>Campos de una línea, compartidos por Create y Update para validarlos con una sola clase base.</summary>
    public interface IDocumentoCompraLinea
    {
        string PlantaCode { get; }
        string CuentaContableCode { get; }
        string? AnexoTipoCode { get; }
        string? AnexoCode { get; }
        string Detalle { get; }
        decimal Cantidad { get; }
        string UnidadMedidaCode { get; }
        decimal PrecioSol { get; }
        string? NumeroGuiaVenta { get; }
        string? ProductoTipoCode { get; }
        string? ProductoCode { get; }
    }
}
