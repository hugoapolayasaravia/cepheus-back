using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;

public sealed class ProgramacionOtrosDetalleResponse
{
    public int Item { get; set; }
    public string NegocioCode { get; set; } = default!;
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public string ProductoName { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal BalanceQuantity { get; set; }
    public decimal OrderQuantity { get; set; }
    public decimal ApprovedQuantity { get; set; }

    /// <summary>Aprobado por Créditos que todavía no está en ninguna programación.</summary>
    public decimal AvailableQuantity { get; set; }
}

public sealed class ProgramacionOtrosResponse
{
    public string Code { get; set; } = default!;
    public string OrdenTrabajoCode { get; set; } = default!;
    public string ClienteCode { get; set; } = default!;
    public string? ClienteName { get; set; }
    public string ObraCode { get; set; } = default!;
    public string ObraName { get; set; } = default!;
    public string? DeliveryAddress { get; set; }
    public string? ResponsibleName { get; set; }
    public string? ResponsiblePhone { get; set; }
    public string? VendedorCode { get; set; }

    public DateTime ProgramDate { get; set; }
    public string ProgramTime { get; set; } = default!;
    public string PlantCode { get; set; } = default!;
    public string? PlantName { get; set; }
    public string? TransportObservations { get; set; }
    public EstadoProgramacion Status { get; set; }

    public decimal Quantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal BalanceQuantity { get; set; }

    /// <summary>true si aún se puede modificar o anular (Pendiente y sin entregas ni guías).</summary>
    public bool CanModify { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    public List<ProgramacionOtrosDetalleResponse> Detalles { get; set; } = new();
}

/// <summary>Fila del listado de programaciones de Otros.</summary>
public sealed class ProgramacionOtrosListadoResponse
{
    public string Code { get; set; } = default!;
    public DateTime ProgramDate { get; set; }
    public string ProgramTime { get; set; } = default!;
    public string OrdenTrabajoCode { get; set; } = default!;
    public string ClienteCode { get; set; } = default!;
    public string? ClienteName { get; set; }
    public string ObraCode { get; set; } = default!;
    public string ObraName { get; set; } = default!;
    public string? VendedorCode { get; set; }
    public string PlantCode { get; set; } = default!;
    public string? PlantName { get; set; }
    public decimal Quantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal BalanceQuantity { get; set; }
    public int Lines { get; set; }
    public string? TransportObservations { get; set; }
    public EstadoProgramacion Status { get; set; }
}

/// <summary>Línea de la OT disponible para programar en Otros.</summary>
public sealed class LineaDisponibleOtrosResponse
{
    public int Item { get; set; }
    public string NegocioCode { get; set; } = default!;
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public string ProductoName { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public decimal OrderQuantity { get; set; }
    public decimal ApprovedQuantity { get; set; }
    public decimal ProgrammedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
}
