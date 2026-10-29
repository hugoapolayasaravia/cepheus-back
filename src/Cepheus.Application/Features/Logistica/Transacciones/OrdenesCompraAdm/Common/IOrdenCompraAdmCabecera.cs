// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/Common/IOrdenCompraAdministracionCabecera.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common
{
    /// <summary>
    /// Campos de cabecera compartidos por Create y Update, para validarlos con una sola
    /// clase base (OrdenCompraAdministracionCabeceraValidator).
    /// </summary>
    public interface IOrdenCompraAdmCabecera
    {
        string ComprobantePagoCode { get; }
        string? EmitirDocumento { get; }
        DateTime FechaEntrega { get; }
        string ProveedorCode { get; }
        string CompradorCode { get; }
        string MonedaCode { get; }
        string LugarEnvioCode { get; }
        string FormaPagoCode { get; }
        string TramiteCode { get; }
        string? Observaciones1 { get; }
        string? Observaciones2 { get; }
        string? NotaCompraCode { get; }
        string UnidadNegocioCode { get; }
        string Motivo { get; }
        decimal NoGravableCompra { get; }
        decimal ServicioCompra { get; }
        decimal IgvExteriorCompra { get; }
    }
}
