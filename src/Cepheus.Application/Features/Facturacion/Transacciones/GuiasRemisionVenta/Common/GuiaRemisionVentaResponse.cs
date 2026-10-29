using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;

public sealed class GuiaRemisionVentaDetalleResponse
{
    public int Line { get; set; }
    public int? OrdenTrabajoItem { get; set; }
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public string ProductoName { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal? TopeQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

public sealed class GuiaRemisionVentaResponse
{
    public string Number { get; set; } = default!;
    public string PlantCode { get; set; } = default!;
    public string? PlantName { get; set; }
    public DateTime IssueDate { get; set; }
    public string? DispatchTime { get; set; }
    public MotivoGuiaRemisionVenta Motive { get; set; }
    public EstadoGuiaRemisionVenta Status { get; set; }

    public string ClienteCode { get; set; } = default!;
    public string? ClienteName { get; set; }
    public string ObraCode { get; set; } = default!;
    public string ObraName { get; set; } = default!;
    public string VendedorCode { get; set; } = default!;

    public string Address { get; set; } = default!;
    public string UbigeoCode { get; set; } = default!;
    public string? UbigeoName { get; set; }
    public string Description { get; set; } = default!;

    public string TransportistaCode { get; set; } = default!;
    public string? TransportistaName { get; set; }
    public string ChoferCode { get; set; } = default!;
    public string? ChoferName { get; set; }
    public TipoVehiculo? VehicleType { get; set; }
    public string? VehicleCode { get; set; }
    public string? LicensePlate { get; set; }

    public FlujoDocumentoAprobacion? DocumentFlow { get; set; }
    public string? DocumentTypeCode { get; set; }
    public string? Document1Number { get; set; }
    public string? Document2Number { get; set; }

    public string? OrdenTrabajoCode { get; set; }
    public TipoProgramacionGuia? ProgramacionType { get; set; }
    public string? ProgramacionCode { get; set; }

    public string FleteCode { get; set; } = default!;
    public string? FleteName { get; set; }
    public decimal FleteAmount { get; set; }

    public string? ReplacesNumber { get; set; }
    public string? DevolutionMotiveCode { get; set; }
    public string? DevolutionActionCode { get; set; }

    public string CurrencyCode { get; set; } = default!;
    public decimal ExchangeRate { get; set; }
    public string? TicketNumber { get; set; }
    public string? PumpCode { get; set; }

    public bool IsPrinted { get; set; }
    public DateTime? PrintedAt { get; set; }
    public bool IsBalance { get; set; }
    public DateTime? CanceledAt { get; set; }
    public string? CanceledBy { get; set; }
    public string? CancelReason { get; set; }

    public decimal Quantity { get; set; }

    /// <summary>Pendiente y sin imprimir (PB: ue_set_btn_mod_dw).</summary>
    public bool CanModify { get; set; }

    /// <summary>Pendiente (PB: ue_set_btn_eli_bor).</summary>
    public bool CanAnnul { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    public List<GuiaRemisionVentaDetalleResponse> Detalles { get; set; } = new();
}

public sealed class GuiaRemisionVentaListadoResponse
{
    public string Number { get; set; } = default!;
    public DateTime IssueDate { get; set; }
    public MotivoGuiaRemisionVenta Motive { get; set; }
    public EstadoGuiaRemisionVenta Status { get; set; }
    public string ClienteCode { get; set; } = default!;
    public string? ClienteName { get; set; }
    public string ObraCode { get; set; } = default!;
    public string ObraName { get; set; } = default!;
    public string PlantCode { get; set; } = default!;
    public string? OrdenTrabajoCode { get; set; }
    public TipoProgramacionGuia? ProgramacionType { get; set; }
    public string? ProgramacionCode { get; set; }
    public string? DocumentTypeCode { get; set; }
    public string? Document1Number { get; set; }
    public decimal Quantity { get; set; }
    public bool IsPrinted { get; set; }
}

public sealed class GuiaProgramacionLineaResponse
{
    public int Item { get; set; }
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public string ProductoName { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public decimal ProgrammedQuantity { get; set; }
    public decimal DeliveredQuantity { get; set; }

    /// <summary>Programado menos entregado: tope de la línea de la guía.</summary>
    public decimal PendingQuantity { get; set; }

    /// <summary>Aprobado por Créditos menos entregado de la línea de la OT.</summary>
    public decimal ApprovedBalance { get; set; }
    public decimal UnitPrice { get; set; }
}

/// <summary>Datos para armar una guía desde una programación: cabecera por defecto y líneas con saldo.</summary>
public sealed class GuiaProgramacionResponse
{
    public TipoProgramacionGuia ProgramacionType { get; set; }
    public string ProgramacionCode { get; set; } = default!;
    public EstadoProgramacion ProgramacionStatus { get; set; }
    public string OrdenTrabajoCode { get; set; } = default!;
    public string ClienteCode { get; set; } = default!;
    public string? ClienteName { get; set; }
    public string ObraCode { get; set; } = default!;
    public string ObraName { get; set; } = default!;
    public string VendedorCode { get; set; } = default!;
    public string PlantCode { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string UbigeoCode { get; set; } = default!;
    public string CurrencyCode { get; set; } = default!;
    public decimal? ExchangeRate { get; set; }
    public FlujoDocumentoAprobacion? DocumentFlow { get; set; }
    public string? DocumentTypeCode { get; set; }
    public string? Document1Number { get; set; }
    public string? Document2Number { get; set; }

    /// <summary>Número que tomaría la guía en la planta (último + 1); null si la numeración de la planta no es válida.</summary>
    public string? NextNumber { get; set; }

    public List<GuiaProgramacionLineaResponse> Lineas { get; set; } = new();
}
