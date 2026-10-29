namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.Common
{
    public class AsientoConsumoResultResponse
    {
        public string PlantaCode { get; set; } = default!;
        public bool Success { get; set; }
        public string? AsientoCodigo { get; set; }
        /// <summary>Líneas de vale contabilizadas.</summary>
        public int Lineas { get; set; }
        /// <summary>Vales distintos contabilizados.</summary>
        public int Vales { get; set; }
        public decimal TotalSol { get; set; }
        public string? Message { get; set; }
    }

    /// <summary>
    /// Fila de la búsqueda de consumo (columnas de CO_consultaconsumo): una línea de vale de salida Procesada.
    /// </summary>
    public class ValeConsumoAsientoItemResponse
    {
        /// <summary>cod_planta del legacy: planta afectada (beneficiada) del vale.</summary>
        public string PlantaAfectadaCode { get; set; } = default!;
        public string ValeCode { get; set; } = default!;
        public string SubCentroCostoCode { get; set; } = default!;
        /// <summary>fecha_pro del legacy (auditoría de creación del vale).</summary>
        public DateTime FechaProceso { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string ArticuloCode { get; set; } = default!;
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
        public string? AsientoContable { get; set; }
        /// <summary>codigo_pla del legacy: planta que entrega.</summary>
        public string PlantaCode { get; set; } = default!;
        public string TipoValeCode { get; set; } = default!;
        public int ItemNumber { get; set; }
    }
}
