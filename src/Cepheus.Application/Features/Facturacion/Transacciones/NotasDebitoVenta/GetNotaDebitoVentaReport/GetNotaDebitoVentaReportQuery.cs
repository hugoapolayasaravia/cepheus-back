using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.GetNotaDebitoVentaReport
{
    /// <summary>
    /// Datos de impresión de la nota de débito (legacy: SP fac_rpt_notadedebito / dw_rpt_notadebito).
    /// </summary>
    public record GetNotaDebitoVentaReportQuery(string PlantaCode, string Number) : IRequest<NotaDebitoVentaReportResponse>;

    public class NotaDebitoVentaReportLine
    {
        public string Description { get; set; } = default!;
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class NotaDebitoVentaReportResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Number { get; set; } = default!;
        public DateTime IssueDate { get; set; }

        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        /// <summary>RUC o DNI del cliente (el legacy muestra el RUC si tiene 11 dígitos; si no, el DNI).</summary>
        public string? ClienteDocument { get; set; }
        public string? ClienteAddress { get; set; }
        public string? ClienteUbigeo { get; set; }

        public string DeliveryAddress { get; set; } = default!;
        public string? DeliveryUbigeo { get; set; }

        /// <summary>FAC o BOL cuando la nota debita un documento; null si no tiene.</summary>
        public string? DocumentLabel { get; set; }
        public string? DocumentNumber { get; set; }

        public string MonedaCode { get; set; } = default!;
        public decimal SubTotal { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal TotalAmount { get; set; }
        /// <summary>Porcentaje de IGV vigente a la fecha de emisión (legacy: fn_Igv).</summary>
        public decimal IgvPercentage { get; set; }

        public string Description { get; set; } = default!;
        public int StatusCode { get; set; }
        public string Status { get; set; } = default!;

        public List<NotaDebitoVentaReportLine> Lines { get; set; } = new();
    }
}
