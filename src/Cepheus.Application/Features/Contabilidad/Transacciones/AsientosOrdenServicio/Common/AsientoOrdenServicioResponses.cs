namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.Common
{
    public class AsientoOrdenServicioResultResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string OrdenServicioCode { get; set; } = default!;
        public bool Success { get; set; }
        public string? AsientoCodigo { get; set; }
        public string? DocumentoComprobantePagoCode { get; set; }
        public string? DocumentoNumero { get; set; }
        public string? Message { get; set; }
    }

    /// <summary>
    /// Fila de la búsqueda de órdenes de servicio (columnas de CO_OrdenServicio). IgvSol, NetoSol y TotalSol son los
    /// importes en soles, con signo negativo en la nota de crédito (columnas IGV, NETO y T_tot del SP).
    /// </summary>
    public class OrdenServicioAsientoItemResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string ComprobantePagoCode { get; set; } = default!;
        public string? ComprobantePagoName { get; set; }
        public string? NumeroDocumento { get; set; }
        public string MonedaCode { get; set; } = default!;
        public DateTime FechaRecepcion { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public decimal Monto { get; set; }
        public decimal NoGravable { get; set; }
        public decimal Renta { get; set; }
        public decimal Fonavi { get; set; }
        public decimal Servicio { get; set; }
        public decimal IgvExterior { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public string FormaPagoCode { get; set; } = default!;
        public string? AsientoContable { get; set; }
        public decimal IgvSol { get; set; }
        public decimal NetoSol { get; set; }
        public decimal TotalSol { get; set; }
    }
}
