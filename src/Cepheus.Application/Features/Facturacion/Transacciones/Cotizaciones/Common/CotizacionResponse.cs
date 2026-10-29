namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common
{
    /// <summary>
    /// Respuesta de cabecera de Cotizacion. No incluye Detalles/Notas/Metrado:
    /// esas colecciones se consultan por separado (GetCotizacionDetallesByCotizacion,
    /// GetCotizacionNotasByCotizacion), mismo criterio que OrdenTrabajoResponse
    /// en Mantenimiento frente a OTRMateriales/OTResponsables.
    /// </summary>
    public class CotizacionResponse
    {
        public string NegocioCode { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string Code { get; set; } = default!;

        public string VendedorCode { get; set; } = default!;
        public DateTime Date { get; set; }
        public string CurrencyCode { get; set; } = default!;
        public string FormaPagoVentaCode { get; set; } = default!;
        public string? TecnicoCode { get; set; }

        public bool AppliesIgv { get; set; }
        public decimal Discount { get; set; }
        public decimal GlobalVolume { get; set; }
        public bool IsEditable { get; set; }
        public string Type { get; set; } = default!;
        public string? MetradoCalculationSystem { get; set; }
        public int WorkDurationMonths { get; set; }

        public string? ClienteCode { get; set; }
        public string? Ruc { get; set; }
        public string ClientName { get; set; } = default!;
        public string? ClientAddress { get; set; }
        public string ClientAddressUbigeoCode { get; set; } = default!;

        public string? ObraCode { get; set; }
        public string WorkName { get; set; } = default!;
        public string ProjectStatus { get; set; } = default!;
        public string WorkAddressUbigeoCode { get; set; } = default!;
        public string WorkAddress { get; set; } = default!;

        public string ContactName { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;
        public string? Reference { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DispatchDate { get; set; }

        public string FleteCode { get; set; } = default!;
        public decimal IgvRate { get; set; }
        public DateTime ProcessDate { get; set; }

        public decimal GrossAmount { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal NetAmount { get; set; }

        public string Status { get; set; } = default!;

        public string? OriginNegocioCode { get; set; }
        public string? OriginYear { get; set; }
        public string? OriginMonth { get; set; }
        public string? OriginCode { get; set; }

        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? CancelReason { get; set; }
        public string? CanceledBy { get; set; }
        public DateTime? CanceledAt { get; set; }

        public bool IsPrinted { get; set; }
        public string? Observations { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
