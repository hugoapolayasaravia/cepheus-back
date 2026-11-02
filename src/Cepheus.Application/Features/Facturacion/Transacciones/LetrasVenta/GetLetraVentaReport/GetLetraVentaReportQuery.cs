using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetraVentaReport
{
    /// <summary>
    /// Datos de impresión de la letra (legacy: reporte "Letra" de w_glb_cre_preview).
    /// El legacy arma el código de impresión como letra + opción (S/N) + secuencia del aval + documento del aval;
    /// aquí <c>AvalSequence</c> elige el aval del cliente (dw_bus_cre_avales) y <c>AvalOption</c> es la opción S/N
    /// (legacy: 'S' imprime con dw_rpt_letras_o, 'N' con dw_rpt_letras; papel 210x110 mm). Con <c>AvalOptional</c> un aval
    /// inexistente no es error (lo usa el lote, que toma el aval 001 si el cliente lo tiene).
    /// </summary>
    public record GetLetraVentaReportQuery(string PlantaCode, string Number, string? AvalSequence = null, string AvalOption = "S", bool AvalOptional = false)
        : IRequest<LetraVentaReportResponse>;

    public class LetraVentaReportDocument
    {
        public string TypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public DateTime DocumentDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string DocumentCurrencyCode { get; set; } = default!;
    }

    public class LetraVentaReportAval
    {
        public string Sequence { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? DocumentTypeCode { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }

    public class LetraVentaReportResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Number { get; set; } = default!;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }

        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public string? ClienteDocumentTypeCode { get; set; }
        public string? ClienteDocument { get; set; }
        public string? ClienteAddress { get; set; }
        public string? ClienteUbigeo { get; set; }
        /// <summary>Solo el distrito (la impresión por lote usa el distrito; la individual, el ubigeo completo).</summary>
        public string? ClienteDistrict { get; set; }
        public string? ClientePhone { get; set; }

        public string? BancoName { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        /// <summary>Importe en soles y en dólares (legacy TotNetoSol_let / TotNetoDol_let).</summary>
        public decimal TotalSoles { get; set; }
        public decimal TotalDollars { get; set; }
        public decimal Interest { get; set; }
        public string? Description { get; set; }

        /// <summary>Monto en letras, ej. "MIL DOSCIENTOS CON 50/100 SOLES".</summary>
        public string AmountInWords { get; set; } = default!;

        /// <summary>Opción S/N de impresión recibida (legacy "opcion"); se devuelve tal cual.</summary>
        public string AvalOption { get; set; } = "S";

        /// <summary>Aval elegido (null si no se eligió ninguno).</summary>
        public LetraVentaReportAval? Aval { get; set; }

        public bool AcceptedByClient { get; set; }
        public string Status { get; set; } = default!;

        /// <summary>Documentos que la letra canjea.</summary>
        public List<LetraVentaReportDocument> Documents { get; set; } = new();
    }
}
