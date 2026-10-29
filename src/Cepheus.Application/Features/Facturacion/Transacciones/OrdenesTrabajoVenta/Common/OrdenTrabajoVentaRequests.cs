using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>
/// Datos que se pueden modificar de la cabecera de una OT de Ventas (PB: dw_1 de w_ges_reg_ordenes_trabajo).
/// El cliente, la obra, el tipo, la OT origen y la cotización se definen al crear y no cambian.
///
/// No se reciben: el código (correlativo global), la etapa, el estado, las banderas de aprobación y
/// el total (suma de las líneas).
/// </summary>
public class OrdenTrabajoVentaUpdateFields
{
    /// <summary>Fecha de despacho (solo fecha). Obligatoria.</summary>
    public DateTime DispatchDate { get; set; }

    /// <summary>Hora de despacho "HH:mm" (hora 01-23, minuto 00-59; el legacy rechaza la hora 00). Obligatoria.</summary>
    public string DispatchTime { get; set; } = default!;

    /// <summary>Si se omite se toma el vendedor de la obra.</summary>
    public string? VendedorCode { get; set; }

    /// <summary>Código de motivo (catálogo MotivoDevolucion). Obligatorio.</summary>
    public string MotivoCode { get; set; } = default!;

    /// <summary>
    /// Moneda (char(1), como el legacy). Si se omite: la de la cotización, cuando la hay.
    /// </summary>
    public string? CurrencyCode { get; set; }

    public string? Observations { get; set; }

    /// <summary>N° de orden de compra del cliente. Obligatorio en OT Normal/Anticipo si el cliente "requiere aprobación de OC".</summary>
    public string? PurchaseOrderNumber { get; set; }

    /// <summary>Observaciones cuando la OT proviene de Hoja de Corte.</summary>
    public string? CutSheetObservations { get; set; }
}

/// <summary>Datos de la cabecera al crear una OT de Ventas.</summary>
public class OrdenTrabajoVentaHeaderRequest : OrdenTrabajoVentaUpdateFields
{
    /// <summary>
    /// Normal, Anticipo o Descuento. Saldo y Consolidacion no se crean a mano: las genera el proceso de
    /// saldos/consolidación de la fase de programación (PB: "El Tipo de Orden de Trabajo Saldo no disponible
    /// en esta opción").
    /// </summary>
    public TipoOrdenTrabajoVenta Type { get; set; }

    public string ClienteCode { get; set; } = default!;
    public string ObraCode { get; set; } = default!;

    /// <summary>Obligatoria solo para el tipo Descuento: la OT de Anticipo que se descuenta.</summary>
    public string? OriginCode { get; set; }

    /// <summary>
    /// Cotización de Venta de origen (opcional, una sola por OT). Las cuatro partes van juntas.
    /// </summary>
    public string? CotizacionNegocioCode { get; set; }
    public string? CotizacionYear { get; set; }
    public string? CotizacionMonth { get; set; }
    public string? CotizacionCode { get; set; }
}

/// <summary>Campos de una línea que no identifican al producto (comunes a alta y modificación).</summary>
public class OrdenTrabajoVentaDetalleFields
{
    public decimal Quantity { get; set; }

    /// <summary>
    /// Precio unitario SIEMPRE positivo. En productos tipo 23 (descuento) el sistema lo guarda negativo
    /// (PB: "if ls_tpr = '23' then ld_pre = ld_pre * -1").
    /// </summary>
    public decimal UnitPrice { get; set; }
}

/// <summary>Línea nueva: producto, negocio, cantidad y precio.</summary>
public sealed class OrdenTrabajoVentaDetalleRequest : OrdenTrabajoVentaDetalleFields
{
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;

    /// <summary>Negocio de la línea (CO, AG, BL, BA, PT, AD...). Se valida contra Comunes.Negocio.</summary>
    public string NegocioCode { get; set; } = default!;
}

/// <summary>Modificación de una línea: el producto y el negocio no cambian (se elimina y se agrega otra).</summary>
public sealed class UpdateOrdenTrabajoVentaDetalleRequest : OrdenTrabajoVentaDetalleFields
{
}

/// <summary>Una Hoja de Corte (año + código) que se exporta a la OT.</summary>
public sealed class HojaCorteReferenceRequest
{
    public string Year { get; set; } = default!;
    public string Code { get; set; } = default!;
}

/// <summary>Precio unitario de un producto de Hoja de Corte (las HC no traen precio).</summary>
public sealed class HojaCortePrecioRequest
{
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public decimal UnitPrice { get; set; }
}
