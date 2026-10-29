namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common
{
    /// <summary>Campos de cabecera compartidos por Create y Update, para validarlos con una sola clase base.</summary>
    public interface IDocumentoCompraCabecera
    {
        string ComprobantePagoCode { get; }
        string NumeroDocumento { get; }
        string ProveedorCode { get; }
        DateTime FechaEmision { get; }
        DateTime FechaRecepcion { get; }
        DateTime FechaVencimiento { get; }
        string FormaPagoCode { get; }
        string MonedaCode { get; }
        DateOnly? TipoCambioFecha { get; }
        decimal NoGravableSol { get; }
        decimal ServicioSol { get; }
        decimal IgvExteriorSol { get; }
        string Glosa { get; }
        string Origen { get; }
        string? Procedencia { get; }
        string? TipoBienCode { get; }
        string? TipoOperacionCode { get; }
        string? OperacionDetraccion { get; }
        string? ConstanciaDetraccion { get; }
        DateTime? FechaConstanciaDetraccion { get; }
        string? NumeroDetraccion { get; }
        DateTime? FechaEmisionDetraccion { get; }
        decimal MontoDetraccionSol { get; }
        string? DocumentoOrigenComprobantePagoCode { get; }
        string? DocumentoOrigenNumero { get; }
        string? OrdenCompraAdministracionCode { get; }
    }
}
