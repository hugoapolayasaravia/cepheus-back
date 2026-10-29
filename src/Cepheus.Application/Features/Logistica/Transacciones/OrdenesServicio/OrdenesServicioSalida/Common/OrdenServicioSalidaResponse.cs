using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;

/// <summary>Cabecera del vale de salida (sin líneas). Es lo que viaja embebido en la respuesta de la Orden de Servicio.</summary>
public class OrdenServicioSalidaResumenResponse
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public EstadoOrdenServicioSalida Estado { get; set; }
    public string EstadoDescripcion { get; set; } = default!;
    public DateTime FechaEntrega { get; set; }
    public string TrabajadorCode { get; set; } = default!;
    public string TrabajadorName { get; set; } = default!;
    /// <summary>Importes en SOLES, derivados de las líneas.</summary>
    public decimal Neto { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public string? AsientoContable { get; set; }
    public string? AprobadoPor { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? ProcesadoPor { get; set; }
    public DateTime? FechaProcesado { get; set; }
}

public sealed class OrdenServicioSalidaDetalleResponse
{
    public int ItemNumber { get; set; }
    public string ArticuloCode { get; set; } = default!;
    public string ArticuloName { get; set; } = default!;
    public string UnidadMedidaCode { get; set; } = default!;
    public string? Glosa { get; set; }
    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Total { get; set; }
    public EstadoOrdenServicioSalidaDetalle Estado { get; set; }
    public string? AsientoContable { get; set; }
    public string? Propiedad01 { get; set; }
    public string TipoValeCode { get; set; } = default!;
    public string TipoValeName { get; set; } = default!;
    public string SubCentroCostoCode { get; set; } = default!;
    public string SubCentroCostoName { get; set; } = default!;
    public string? SubCentroEjecutorCode { get; set; }
    public string? SubCentroEjecutorName { get; set; }
    public string? OrdenTrabajoCode { get; set; }
    public string PlantaAfectadaCode { get; set; } = default!;
}

/// <summary>Vale de salida completo: cabecera + líneas.</summary>
public sealed class OrdenServicioSalidaResponse : OrdenServicioSalidaResumenResponse
{
    public string PlantaName { get; set; } = default!;
    public string? CreatedBy { get; set; }
    public List<OrdenServicioSalidaDetalleResponse> Detalles { get; set; } = new();
}

/// <summary>Fila del listado paginado de vales de salida.</summary>
public sealed class OrdenServicioSalidaListadoResponse
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public EstadoOrdenServicioSalida Estado { get; set; }
    public string EstadoDescripcion { get; set; } = default!;
    public string TrabajadorCode { get; set; } = default!;
    public string TrabajadorName { get; set; } = default!;
    public DateTime FechaProceso { get; set; }
    public DateTime FechaEntrega { get; set; }
    public decimal Neto { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
}
