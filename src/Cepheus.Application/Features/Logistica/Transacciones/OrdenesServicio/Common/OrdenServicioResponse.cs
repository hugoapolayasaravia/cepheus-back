using Cepheus.Domain.Logistica.Enum;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;

public sealed class OrdenServicioDetalleResponse
{
    public int ItemNumber { get; set; }
    public string ArticuloCode { get; set; } = default!;
    public string ArticuloName { get; set; } = default!;
    public string UnidadMedidaCode { get; set; } = default!;
    public string? Glosa { get; set; }
    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public EstadoOrdenServicioDetalle Estado { get; set; }
    public string TipoValeCode { get; set; } = default!;
    public string TipoValeName { get; set; } = default!;
    public string SubCentroCostoCode { get; set; } = default!;
    public string SubCentroCostoName { get; set; } = default!;
    public string? SubCentroEjecutorCode { get; set; }
    public string? SubCentroEjecutorName { get; set; }
    public string? OrdenTrabajoCode { get; set; }
    public string PlantaAfectadaCode { get; set; } = default!;
}

public sealed class OrdenServicioResponse
{
    public string PlantaCode { get; set; } = default!;
    public string PlantaName { get; set; } = default!;
    public string Code { get; set; } = default!;
    public EstadoOrdenServicio Estado { get; set; }
    public string EstadoDescripcion { get; set; } = default!;

    public string ComprobantePagoCode { get; set; } = default!;
    public string ComprobantePagoName { get; set; } = default!;
    public string? NumeroDocumento { get; set; }

    public string ProveedorCode { get; set; } = default!;
    public string ProveedorName { get; set; } = default!;
    public string MonedaCode { get; set; } = default!;
    public string FormaPagoCode { get; set; } = default!;
    public string FormaPagoName { get; set; } = default!;

    public DateTime FechaRecepcion { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal TipoCambio { get; set; }

    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal Monto { get; set; }
    public decimal NoGravable { get; set; }
    public decimal Renta { get; set; }
    public decimal Fonavi { get; set; }
    public decimal Servicio { get; set; }
    public decimal IgvExterior { get; set; }

    public string? AsientoContable { get; set; }
    public string? AprobadoPor { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? ProcesadoPor { get; set; }
    public DateTime? FechaProcesado { get; set; }

    public string CompradorCode { get; set; } = default!;
    public string CompradorName { get; set; } = default!;
    public string LugarEnvioCode { get; set; } = default!;
    public string LugarEnvioName { get; set; } = default!;
    public string TramiteCode { get; set; } = default!;
    public string TramiteName { get; set; } = default!;
    public string? NotaCompraCode { get; set; }
    public string? NotaCompraName { get; set; }
    public string UnidadNegocioCode { get; set; } = default!;
    public string? UnidadNegocioName { get; set; }

    /// <summary>Vale de salida asociado (cabecera). Las líneas del vale: GET ordenes-servicio-salida/{planta}/{codigo}.</summary>
    public OrdenServicioSalidaResumenResponse ValeSalida { get; set; } = default!;

    public string? Observaciones1 { get; set; }
    public string? Observaciones2 { get; set; }

    public string? CreatedBy { get; set; }
    public List<OrdenServicioDetalleResponse> Detalles { get; set; } = new();
}

/// <summary>
/// Fila del listado paginado: campos del SP Logi_sp_Listado_MOrdenServicio_I (planta, código, estado,
/// proveedor, moneda, fecha de proceso, tipo de cambio, total, fecha de emisión, número de documento, comprobante).
/// </summary>
public sealed class OrdenServicioListadoResponse
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public EstadoOrdenServicio Estado { get; set; }
    public string EstadoDescripcion { get; set; } = default!;
    public string ProveedorCode { get; set; } = default!;
    public string ProveedorName { get; set; } = default!;
    public string MonedaCode { get; set; } = default!;
    public DateTime FechaProceso { get; set; }
    public decimal TipoCambio { get; set; }
    public decimal Total { get; set; }
    public DateTime FechaEmision { get; set; }
    public string? NumeroDocumento { get; set; }
    public string ComprobantePagoCode { get; set; } = default!;
    public string ComprobantePagoName { get; set; } = default!;
}
