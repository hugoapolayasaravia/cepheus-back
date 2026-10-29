using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

public sealed class OrdenTrabajoVentaDetalleResponse
{
    public int Item { get; set; }
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public string ProductoName { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public string UnitName { get; set; } = default!;
    public string NegocioCode { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal ApprovedQuantity { get; set; }
    public decimal ProgrammedQuantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public EstadoDetalleOrdenTrabajoVenta Status { get; set; }

    /// <summary>
    /// true si la línea proviene de una Hoja de Corte vigente (productos 16/18 de una OT con HC): no se
    /// edita ni se elimina por separado.
    /// </summary>
    public bool IsLockedByHojaCorte { get; set; }
}

public sealed class OrdenTrabajoVentaHojaCorteResponse
{
    public string Year { get; set; } = default!;
    public string Code { get; set; } = default!;
    public EstadoHojaCorte Status { get; set; }
    public string? Sector { get; set; }
    public string? Floor { get; set; }
}

public sealed class OrdenTrabajoVentaAprobacionResponse
{
    public string? CreditsObservations { get; set; }
    public string? Document1Number { get; set; }
    public TipoDocumentoAprobacion DocumentType { get; set; }
    public decimal ApprovedQuantity { get; set; }
    public DateTime ApprovedAt { get; set; }
    public TipoPagoAprobacion PaymentType { get; set; }
    public EstadoCreditoAprobacion CreditStatus { get; set; }
    public FlujoDocumentoAprobacion DocumentFlow { get; set; }
    public string ValorizationCode { get; set; } = default!;
    public DiaSemana? ValorizationWeekday { get; set; }
    public int? ValorizationDays { get; set; }
    public string? Document2Number { get; set; }
    public bool HasTwoDocuments { get; set; }
    public string? Plant1Code { get; set; }
    public string? Plant1Name { get; set; }
    public string? Plant2Code { get; set; }
    public string? Plant2Name { get; set; }
    public string? GuideMotivoCode { get; set; }
    public string? PaymentMethodCode { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
    public string? PaymentDocumentNumber { get; set; }
    public string FormaPagoVentaCode { get; set; } = default!;
    public string FormaPagoVentaName { get; set; } = default!;
}

public sealed class OrdenTrabajoVentaResponse
{
    public string Code { get; set; } = default!;
    public TipoOrdenTrabajoVenta Type { get; set; }
    public EtapaOrdenTrabajoVenta Stage { get; set; }
    public EstadoOrdenTrabajoVenta Status { get; set; }
    public string StatusDescription { get; set; } = default!;

    public string ClienteCode { get; set; } = default!;
    public string ClientName { get; set; } = default!;
    public string ObraCode { get; set; } = default!;
    public string WorkName { get; set; } = default!;
    public string WorkAddress { get; set; } = default!;
    public string? ResponsibleName { get; set; }
    public string? ResponsiblePhone { get; set; }

    public DateTime? DispatchDate { get; set; }
    public string? DispatchTime { get; set; }

    public string VendedorCode { get; set; } = default!;
    public string VendedorName { get; set; } = default!;
    public string CurrencyCode { get; set; } = default!;
    public string MotivoCode { get; set; } = default!;
    public string MotivoName { get; set; } = default!;

    public string? Observations { get; set; }
    public string CreditObservations { get; set; } = default!;
    public string? CutSheetObservations { get; set; }

    /// <summary>Total sin IGV (suma de las líneas no anuladas).</summary>
    public decimal Total { get; set; }
    public decimal IgvRate { get; set; }
    public decimal TotalWithIgv { get; set; }

    /// <summary>
    /// Tipo de productos de la OT (PB: Tipo_prd), calculado de los negocios de las líneas en orden de
    /// aparición y sin repetir: CO=C, AG=A, BL=B, BA=E, PT=V, AD=Q (ej. "CV"). Vacío si no hay líneas.
    /// </summary>
    public string ProductTypes { get; set; } = string.Empty;

    /// <summary>OT origen (copia, anticipo que se descuenta, o saldo).</summary>
    public string? OriginCode { get; set; }

    public string? CotizacionNegocioCode { get; set; }
    public string? CotizacionYear { get; set; }
    public string? CotizacionMonth { get; set; }
    public string? CotizacionCode { get; set; }

    public string? PurchaseOrderNumber { get; set; }

    public bool IsModifiable { get; set; }
    public DateTime? SentToApprovalAt { get; set; }

    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalObservations { get; set; }
    public string? ApprovalDelayReason { get; set; }

    public bool PassedToProduction { get; set; }
    public DateTime? ProductionPassDate { get; set; }
    public string? ProductionPassReason { get; set; }

    public bool SpecialApprovalGranted { get; set; }
    public string? SpecialApprovalBy { get; set; }
    public DateTime? SpecialApprovalAt { get; set; }
    public string? SpecialApprovalObservations { get; set; }

    public string? CanceledBy { get; set; }
    public DateTime? CanceledAt { get; set; }
    public string? CancelReason { get; set; }

    public string? InactivatedBy { get; set; }
    public DateTime? InactivatedAt { get; set; }
    public string? InactivationReason { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public List<OrdenTrabajoVentaDetalleResponse> Detalles { get; set; } = new();
    public List<OrdenTrabajoVentaHojaCorteResponse> HojasCorte { get; set; } = new();
    public OrdenTrabajoVentaAprobacionResponse? Aprobacion { get; set; }
}

/// <summary>Fila del listado (columnas de ges_ordenTrabajo).</summary>
public sealed class OrdenTrabajoVentaListadoResponse
{
    public string Code { get; set; } = default!;
    public DateTime Date { get; set; }
    public string Time { get; set; } = default!;
    public string ClienteCode { get; set; } = default!;
    public string ClientName { get; set; } = default!;
    public string ObraCode { get; set; } = default!;
    public string VendedorAbbreviation { get; set; } = default!;
    public decimal Total { get; set; }
    public decimal TotalWithIgv { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public TipoOrdenTrabajoVenta Type { get; set; }
    public EtapaOrdenTrabajoVenta Stage { get; set; }
    public EstadoOrdenTrabajoVenta Status { get; set; }

    /// <summary>"Atendido 100%" si está cerrada; en otro caso el nombre del estado.</summary>
    public string StatusDescription { get; set; } = default!;

    /// <summary>A Activo | I Inactivo | C Cerrado | T Cerrado definitivo (PB: estado1).</summary>
    public string StatusLetter { get; set; } = default!;

    /// <summary>N Normal | D Descuento | A Anticipo | S Saldo (PB: dato).</summary>
    public string TypeLetter { get; set; } = default!;

    /// <summary>C Contado | R Crédito | A Anticipo | D Descuento (PB: datod).</summary>
    public string PaymentLetter { get; set; } = default!;

    public string ProductTypes { get; set; } = string.Empty;
    public string? OriginCode { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalObservations { get; set; }
    public bool IsModifiable { get; set; }
    public bool PassedToProduction { get; set; }
    public string? CreatedBy { get; set; }
}
