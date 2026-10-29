using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common
{
    /// <summary>Resultado de generar (o eliminar) el asiento de una nota de ingreso.</summary>
    public class AsientoNotaIngresoResultResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string NotaIngresoCode { get; set; } = default!;
        public bool Success { get; set; }
        /// <summary>Código de 13 caracteres del asiento generado (o vinculado).</summary>
        public string? AsientoCodigo { get; set; }
        public string? DocumentoComprobantePagoCode { get; set; }
        public string? DocumentoNumero { get; set; }
        /// <summary>True si el documento ya existía con asiento y solo se vinculó a la nota de ingreso.</summary>
        public bool Vinculado { get; set; }
        public string? Message { get; set; }
    }

    public class GenerarAsientosMasivoResponse
    {
        public int Total { get; set; }
        public int Generados { get; set; }
        public int Fallidos { get; set; }
        public List<AsientoNotaIngresoResultResponse> Resultados { get; set; } = new();
    }

    /// <summary>
    /// Fila de la búsqueda de notas de ingreso (columnas de CO_Ingresos). IgvSol, NetoSol y TotalSol son los
    /// importes en soles con signo negativo para la nota de crédito (columnas IGV, NETO y T_tot del SP).
    /// </summary>
    public class NotaIngresoAsientoItemResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string ComprobantePagoCode { get; set; } = default!;
        public string? ComprobantePagoName { get; set; }
        public string? NumeroDocumento { get; set; }
        public string MonedaCode { get; set; } = default!;
        public DateTime FechaRecepcion { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaDocumento { get; set; }
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
        public string? NumeroReferencia { get; set; }
        public OrigenNotaIngreso Origen { get; set; }
        public decimal IgvSol { get; set; }
        public decimal NetoSol { get; set; }
        public decimal TotalSol { get; set; }
    }
}
