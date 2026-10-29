namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common
{
    /// <summary>
    /// Línea de entrada. Para líneas de origen NotaIngreso u OrdenServicio, lo no indicado se toma del
    /// documento de Logística; para líneas manuales los campos obligatorios se validan al registrar.
    /// Los impuestos no indicados se calculan según las banderas del comprobante.
    /// </summary>
    public record CajaChicaDetalleInput(
        string? Origin,
        string? PlantaCode,
        string? ReferenceCode,
        string? ComprobantePagoCode,
        string? NumeroDocumento,
        DateTime? FechaEmision,
        string? MonedaCode,
        decimal? TipoCambio,
        decimal? Cantidad,
        decimal? PrecioUnidad,
        decimal? Igv,
        decimal? NoGravable,
        decimal? Renta,
        decimal? Fonavi,
        decimal? Servicio,
        decimal? IgvExterior,
        string? CuentaContableCode,
        string? AnexoCode,
        string? Concepto,
        string? ProveedorCode,
        string? DocumentoReferenciaComprobantePagoCode,
        string? DocumentoReferenciaNumero,
        DateTime? DocumentoReferenciaFecha
    );
}
