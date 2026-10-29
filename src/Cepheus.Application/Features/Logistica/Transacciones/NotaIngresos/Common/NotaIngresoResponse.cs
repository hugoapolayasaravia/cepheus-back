using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;

public sealed class NotaIngresoDetalleResponse
{
    public int ItemNumber { get; set; }
    public string ArticuloCode { get; set; } = default!;
    public string ArticuloName { get; set; } = default!;
    public string UnidadMedidaCode { get; set; } = default!;
    public string? PedidoCode { get; set; }
    public string? GuiaCode { get; set; }
    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public EstadoNotaIngresoDetalle Estado { get; set; }
}

public sealed class NotaIngresoResponse
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public CondicionNotaIngreso Condicion { get; set; }
    public OrigenNotaIngreso Origen { get; set; }
    public string? OrdenCompraCode { get; set; }
    public string? ImportacionCode { get; set; }

    public string ComprobantePagoCode { get; set; } = default!;
    public string ComprobantePagoName { get; set; } = default!;

    public string? MotivoDevolucionCode { get; set; } = default!;
    public string? MotivoDevolucionName { get; set; }

    public string? NumeroDocumento { get; set; }
    public string? NumeroGuia { get; set; }
    public string? NumeroReferencia { get; set; }
    public string? ComprobantePagoReferenciaCode { get; set; }

    public string ProveedorCode { get; set; } = default!;
    public string ProveedorName { get; set; } = default!;
    public string MonedaCode { get; set; } = default!;
    public string FormaPagoCode { get; set; } = default!;
    public string FormaPagoName { get; set; } = default!;

    public DateTime FechaRecepcion { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime FechaProceso { get; set; }
    public DateTime? FechaDocumento { get; set; }

    public decimal TipoCambio { get; set; }
    public EstadoNotaIngreso Estado { get; set; }

    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal Monto { get; set; }
    public decimal NoGravable { get; set; }
    public decimal Renta { get; set; }
    public decimal Fonavi { get; set; }
    public decimal Servicio { get; set; }
    public decimal IgvExterior { get; set; }

    public string? AsientoContable { get; set; }
    public string? CodigoVale { get; set; }
    public string? CreatedBy { get; set; }

    public List<NotaIngresoDetalleResponse> Detalles { get; set; } = new();
}

/// <summary>
/// Fila del listado paginado. Campos de salida de Logi_sp_Listado_MNotaIngresos:
/// planta, código, descripción de estado, proveedor, moneda, fecha de proceso, tipo de cambio,
/// total, fecha de emisión, número y tipo de documento. (La columna SAtencion del SP es siempre
/// vacía y no se expone.)
/// </summary>
public sealed class NotaIngresoListadoResponse
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public EstadoNotaIngreso Estado { get; set; }
    public string EstadoDescripcion { get; set; } = default!;
    public string ProveedorCode { get; set; } = default!;
    public string ProveedorName { get; set; } = default!;
    public string MonedaCode { get; set; } = default!;
    public DateTime FechaProceso { get; set; }
    public decimal TipoCambio { get; set; }
    public decimal Total { get; set; }
    public DateTime FechaEmision { get; set; }
    public string? NumeroDocumento { get; set; }
    public int ComprobantePagoId { get; set; }
    public string ComprobantePagoCode { get; set; } = default!;
}
