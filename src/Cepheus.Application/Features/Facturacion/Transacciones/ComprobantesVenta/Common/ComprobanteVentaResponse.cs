using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;

public sealed class ComprobanteVentaDetalleResponse
{
    public int Line { get; set; }
    public string? GuiaNumber { get; set; }
    public int? GuiaLine { get; set; }
    public string? OrdenTrabajoCode { get; set; }
    public int? OrdenTrabajoItem { get; set; }
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public string ProductoName { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

public sealed class ComprobanteVentaResponse
{
    public string TypeCode { get; set; } = default!;
    public string Number { get; set; } = default!;
    public string PlantCode { get; set; } = default!;
    public string? PlantName { get; set; }
    public string ExecutingPlantCode { get; set; } = default!;
    public DateTime IssueDate { get; set; }

    public string ClienteCode { get; set; } = default!;
    public string? ClienteName { get; set; }
    public string? ClienteDocumentNumber { get; set; }
    public string ObraCode { get; set; } = default!;
    public string ObraName { get; set; } = default!;
    public string VendedorCode { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string UbigeoCode { get; set; } = default!;
    public string? UbigeoName { get; set; }

    public string PaymentFormCode { get; set; } = default!;
    public string? PaymentFormName { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public decimal ExchangeRate { get; set; }

    public string? OrdenTrabajoCode { get; set; }
    public OrigenComprobanteVenta Origin { get; set; }
    public FlujoDocumentoAprobacion? DocumentFlow { get; set; }
    public bool IsFreeTransfer { get; set; }
    public string? Glosa { get; set; }

    public decimal IgvPercentage { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal IgvAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalSoles { get; set; }

    public EstadoComprobanteVenta Status { get; set; }
    public bool IsPrinted { get; set; }
    public DateTime? PrintedAt { get; set; }
    public string? ProvisionEntry { get; set; }
    public string? DetractionDocument { get; set; }
    public decimal DetractionAmount { get; set; }
    public string? CashDocument { get; set; }

    public DateTime? CanceledAt { get; set; }
    public string? CanceledBy { get; set; }
    public string? CancelReason { get; set; }
    public string? ReplacedByNumber { get; set; }

    /// <summary>Pendiente y sin provisión contable (PB: ue_set_btn_mod_dw solo con estado Pendiente).</summary>
    public bool CanModify { get; set; }

    /// <summary>Pendiente o Facturado, sin asiento de provisión.</summary>
    public bool CanAnnul { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    public List<ComprobanteVentaDetalleResponse> Detalles { get; set; } = new();
}

public sealed class ComprobanteVentaListadoResponse
{
    public string TypeCode { get; set; } = default!;
    public string Number { get; set; } = default!;
    public DateTime IssueDate { get; set; }
    public string ClienteCode { get; set; } = default!;
    public string? ClienteName { get; set; }
    public string ObraCode { get; set; } = default!;
    public string ObraName { get; set; } = default!;
    public string PlantCode { get; set; } = default!;
    public string? OrdenTrabajoCode { get; set; }
    public OrigenComprobanteVenta Origin { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public EstadoComprobanteVenta Status { get; set; }
    public bool IsPrinted { get; set; }
    public int Lines { get; set; }
}

public sealed class GuiaPendienteFacturarResponse
{
    public string Number { get; set; } = default!;
    public DateTime IssueDate { get; set; }
    public string PlantCode { get; set; } = default!;
    public string ClienteCode { get; set; } = default!;
    public string ObraCode { get; set; } = default!;
    public string? OrdenTrabajoCode { get; set; }
    public string? DocumentTypeCode { get; set; }
    public string? Document1Number { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public decimal Quantity { get; set; }

    /// <summary>Suma cantidad x precio de las líneas (sin IGV), en la moneda de la guía.</summary>
    public decimal Amount { get; set; }
}
