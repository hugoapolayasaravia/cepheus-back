using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;

public sealed class ValeDetalleResponse
{
    public int ItemNumber { get; set; }
    public string ArticuloCode { get; set; } = default!;
    public string ArticuloName { get; set; } = default!;
    public string UnidadMedidaCode { get; set; } = default!;
    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Total { get; set; }
    public EstadoValeDetalle Estado { get; set; }
    public string? Propiedad01 { get; set; }
    public DateTime? MaterialOtFechaProceso { get; set; }
}

public sealed class ValeResponse
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;

    public string TipoValeCode { get; set; } = default!;
    public string TipoValeName { get; set; } = default!;

    public EstadoVale Estado { get; set; }

    public DateTime FechaEntrega { get; set; }
    public DateTime FechaProceso { get; set; }

    public string SubCentroCostoCode { get; set; } = default!;
    public string SubCentroCostoName { get; set; } = default!;
    public string? CentroCostoCode { get; set; }
    public string? CentroCostoName { get; set; }

    public string? SubCentroEjecutorCode { get; set; }
    public string? SubCentroEjecutorName { get; set; }

    public string TrabajadorCode { get; set; } = default!;
    public string TrabajadorName { get; set; } = default!;

    public string? OrdenTrabajoCode { get; set; }
    public string? OrdenTrabajoDescription { get; set; }
    public string? EquipoCode { get; set; }

    public string UnidadNegocioCode { get; set; } = default!;
    public string? UnidadNegocioName { get; set; }

    public string PlantaAfectadaCode { get; set; } = default!;
    public string PlantaAfectadaName { get; set; } = default!;

    public decimal Neto { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }

    public string? AsientoContable { get; set; }

    public string? AprobadoPor { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? ProcesadoPor { get; set; }
    public DateTime? FechaProcesado { get; set; }
    public string? AnuladoPor { get; set; }
    public DateTime? FechaAnulacion { get; set; }

    public bool Preparado { get; set; }
    public string? CreatedBy { get; set; }

    public List<ValeDetalleResponse> Detalles { get; set; } = new();
}

/// <summary>
/// Fila del listado paginado. Campos de salida de Logi_sp_Listado_MVales.
/// </summary>
public sealed class ValeListadoResponse
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public EstadoVale Estado { get; set; }
    public string EstadoDescripcion { get; set; } = default!;
    public DateTime FechaProceso { get; set; }
    public decimal Neto { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public string SubCentroCostoCode { get; set; } = default!;
    public string SubCentroCostoName { get; set; } = default!;
    public string? CentroCostoName { get; set; }
    public string TipoValeCode { get; set; } = default!;
    public string TipoValeName { get; set; } = default!;
    public string? Usuario { get; set; }
    public string? OrdenTrabajoCode { get; set; }
    public string? OrdenTrabajoDescription { get; set; }
    public string? EquipoCode { get; set; }
    public string TrabajadorCode { get; set; } = default!;
    public string TrabajadorName { get; set; } = default!;
    public string PlantaAfectadaCode { get; set; } = default!;
    public string PlantaAfectadaName { get; set; } = default!;
    public string? AprobadoPor { get; set; }
    public DateTime FechaEntrega { get; set; }
    public string UnidadNegocioCode { get; set; } = default!;
    public bool Preparado { get; set; }
}

/// <summary>Datos del artículo para armar una línea (saldos, precio, horómetro).</summary>
public sealed class ValeArticuloInfoResponse
{
    public string ArticuloCode { get; set; } = default!;
    public string ArticuloName { get; set; } = default!;
    public string UnidadMedidaCode { get; set; } = default!;
    public decimal Stock { get; set; }
    public decimal Reservado { get; set; }
    public decimal Disponible { get; set; }

    /// <summary>Precio promedio vigente del artículo en la planta (0 si no está registrado).</summary>
    public decimal Precio { get; set; }

    public bool RequiresHorometro { get; set; }

    /// <summary>Último horómetro procesado del artículo para el mismo centro / subcentro de costo.</summary>
    public string? HorometroAnterior { get; set; }

    /// <summary>Null si no se indicó tipo de vale.</summary>
    public bool? PermitidoParaTipoVale { get; set; }
}

public sealed class ValeMaterialOtResponse
{
    public string ArticuloCode { get; set; } = default!;
    public string ArticuloName { get; set; } = default!;
    public string UnidadMedidaCode { get; set; } = default!;
    public DateTime FechaProceso { get; set; }
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal CostoTotal { get; set; }
    public string? EstadoCode { get; set; }
}

public sealed class ValeOrdenTrabajoMaterialesResponse
{
    public string PlantaCode { get; set; } = default!;
    public string OrdenTrabajoCode { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ResponsableCode { get; set; } = default!;
    public string? SubCentroCostoCode { get; set; }
    public string? SubCentroEjecutorCode { get; set; }
    public string EquipoCode { get; set; } = default!;
    public List<ValeMaterialOtResponse> Materiales { get; set; } = new();
}
