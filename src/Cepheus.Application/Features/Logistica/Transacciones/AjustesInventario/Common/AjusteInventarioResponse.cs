using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;

public sealed class AjusteInventarioDetalleResponse
{
    public int ItemNumber { get; set; }
    public string ArticuloCode { get; set; } = default!;
    public string ArticuloName { get; set; } = default!;
    public string UnidadMedidaCode { get; set; } = default!;
    public TipoAjusteInventario Tipo { get; set; }
    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Total { get; set; }
    public EstadoAjusteInventarioDetalle Estado { get; set; }
}

public sealed class AjusteInventarioResponse
{
    public string PlantaCode { get; set; } = default!;
    public string PlantaName { get; set; } = default!;
    public string Code { get; set; } = default!;
    public EstadoAjusteInventario Estado { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaEntrega { get; set; }
    public DateTime FechaProceso { get; set; }
    public decimal Neto { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public string? AsientoContable { get; set; }
    public string? CreatedBy { get; set; }
    public List<AjusteInventarioDetalleResponse> Detalles { get; set; } = new();
}

/// <summary>Fila del listado paginado. Campos de salida de Logi_sp_Listado_MAjustes.</summary>
public sealed class AjusteInventarioListadoResponse
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public EstadoAjusteInventario Estado { get; set; }
    public string EstadoDescripcion { get; set; } = default!;
    public DateTime FechaProceso { get; set; }
    public decimal Neto { get; set; }
    public string? Usuario { get; set; }
}

/// <summary>Datos del artículo para armar una línea (PB: ue_saldos de dw_6).</summary>
public sealed class AjusteInventarioArticuloInfoResponse
{
    public string ArticuloCode { get; set; } = default!;
    public string ArticuloName { get; set; } = default!;
    public string UnidadMedidaCode { get; set; } = default!;
    public decimal Stock { get; set; }

    /// <summary>Faltantes pendientes de otros ajustes (los vales pendientes se suman cuando exista el módulo Vale).</summary>
    public decimal ReservadoPorAjustes { get; set; }

    /// <summary>Precio promedio vigente del artículo en la planta (0 si no está registrado). Es el precio de la línea.</summary>
    public decimal Precio { get; set; }
}
