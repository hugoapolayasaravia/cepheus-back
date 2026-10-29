using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetReporteDetraccion
{
    /// <summary>Reporte de la detracción de adquiriente de un lote (legacy: rpt_com_detracion_adquiriente).</summary>
    public record GetReporteDetraccionQuery(string Year, string Period, string LotNumber)
        : IRequest<ReporteDetraccionResponse>;

    public class ReporteDetraccionRow
    {
        public int ItemNumber { get; set; }
        public string AccountNumber { get; set; } = default!;
        public string ProveedorRuc { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public string TipoOperacionCode { get; set; } = default!;
        public string? TipoOperacionName { get; set; }
        public string TipoBienCode { get; set; } = default!;
        public string? TipoBienName { get; set; }
        public decimal DetractionRate { get; set; }
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public DateTime ProcessDate { get; set; }
        public decimal TotalSol { get; set; }
        public decimal DetraccionSol { get; set; }
        public bool IncludeInExport { get; set; }
    }

    public class ReporteDetraccionResponse
    {
        public string Year { get; set; } = default!;
        public string Period { get; set; } = default!;
        public string LotNumber { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ReporteDetraccionRow> Rows { get; set; } = new();

        /// <summary>Suma de las detracciones de todos los documentos del lote.</summary>
        public decimal TotalDetraccionSol { get; set; }

        /// <summary>Suma de lo que se deposita (solo documentos marcados para exportar).</summary>
        public decimal TotalDepositoSol { get; set; }
    }
}
