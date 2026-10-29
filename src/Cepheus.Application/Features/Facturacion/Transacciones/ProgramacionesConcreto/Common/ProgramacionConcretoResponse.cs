using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;

public sealed class ProgramacionConcretoDetalleResponse
{
    public int Item { get; set; }
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public string ProductoName { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal BalanceQuantity { get; set; }
    public string? PumpCode { get; set; }
    public byte? Support { get; set; }

    /// <summary>Cantidad de la línea en la OT y la aprobada por Créditos.</summary>
    public decimal OrderQuantity { get; set; }
    public decimal ApprovedQuantity { get; set; }

    /// <summary>Aprobado por Créditos que todavía no está en ninguna programación (disponible para esta u otra).</summary>
    public decimal AvailableQuantity { get; set; }
}

public sealed class ProgramacionConcretoResponse
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
    public string ElementoCode { get; set; } = default!;
    public string? ElementoName { get; set; }
    public string? TransportObservations { get; set; }
    public bool RequiresLabService { get; set; }
    public EstadoProgramacion Status { get; set; }

    /// <summary>Cantidad de concreto (línea tipo 01) o, si no hay, la suma de las líneas.</summary>
    public decimal Quantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal BalanceQuantity { get; set; }

    /// <summary>true si aún se puede modificar o anular (Pendiente y sin entregas ni guías).</summary>
    public bool CanModify { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    public List<ProgramacionConcretoDetalleResponse> Detalles { get; set; } = new();
}

/// <summary>Fila del listado de programaciones de Concreto (ges_programacion_ordenTrabajo).</summary>
public sealed class ProgramacionConcretoListadoResponse
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
    public string ElementoName { get; set; } = default!;
    public string? ProductoName { get; set; }
    public decimal Quantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal BalanceQuantity { get; set; }
    public string? PumpCode { get; set; }
    public byte? Support { get; set; }
    public bool RequiresLabService { get; set; }
    public string? TransportObservations { get; set; }
    public EstadoProgramacion Status { get; set; }
}

/// <summary>Línea de la OT disponible para programar (dw_ges_adicional_x_programa_pendiente).</summary>
public sealed class LineaDisponibleResponse
{
    public int Item { get; set; }
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public string ProductoName { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public decimal OrderQuantity { get; set; }
    public decimal ApprovedQuantity { get; set; }
    public decimal ProgrammedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
}
