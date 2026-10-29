using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Mantenimiento.Maestros;
using Cepheus.Domain.Mantenimiento.Transacciones;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Línea de Orden de Servicio.
///
/// Legacy: dbo.MOrdenServicioDetI (+ MOrdenServicioDetS, que repetía las mismas líneas).
/// La PK legacy incluía Codigo_Ped y Codigo_Gui (siempre vacíos en servicios) además del ítem; se usa
/// la misma PK que NotaIngresoDetalle / PedidoDetalle: (PlantaCode, OrdenServicioCode, ItemNumber).
/// A diferencia de la nota de ingreso, un mismo artículo SÍ puede repetirse en la orden (el legacy
/// tenía comentada la validación de duplicados).
///
///   Codigo_Art    -> ArticuloCode (solo familia 50, servicios)
///   Item_Art      -> ItemNumber (correlativo 1..n dentro de la orden)
///   Glosa_Art     -> Glosa (detalle del servicio)
///   Cantidad_Art  -> Cantidad
///   Precio_Art    -> Precio
///   Descuento_Art -> Descuento: PORCENTAJE (0-100), igual que el legacy
///                    (Total = Cantidad × Precio × (1 − Descuento/100))
///   Total_Art     -> Total
///   Codigo_Est    -> Estado
///   Codigo_tva    -> TipoValeCode (FK Logistica.Catalogos.TipoVale)
///   Codigo_Scc    -> SubCentroCostoCode (FK Logistica.Maestros.SubCentroCosto)
///   Codigo_Sce    -> SubCentroEjecutorCode (FK Mantenimiento.Maestros.SubCentroEjecutor, opcional)
///   Codigo_Otr    -> OrdenTrabajoCode (opcional; FK compuesta Planta + OT)
///   Cod_Planta    -> PlantaAfectadaCode (planta del sub centro de costo; la fija el servidor)
///   Codigo_Uni    -> no se guarda: se toma del artículo (Articulo.UnidadMedidaCode)
///   Cantidad_Ent  -> no aplica a servicios
/// </summary>
public class OrdenServicioDetalle : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public string OrdenServicioCode { get; set; } = default!;
    public OrdenServicio OrdenServicio { get; set; } = default!;

    public int ItemNumber { get; set; }

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

    public string? Glosa { get; set; }

    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    /// <summary>Porcentaje 0-100.</summary>
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }

    public EstadoOrdenServicioDetalle Estado { get; set; } = EstadoOrdenServicioDetalle.Pendiente;

    public string TipoValeCode { get; set; } = default!;
    public TipoVale TipoVale { get; set; } = default!;

    public string SubCentroCostoCode { get; set; } = default!;
    public SubCentroCosto SubCentroCosto { get; set; } = default!;

    public string? SubCentroEjecutorCode { get; set; }
    public SubCentroEjecutor? SubCentroEjecutor { get; set; }

    public string? OrdenTrabajoCode { get; set; }
    public OrdenTrabajo? OrdenTrabajo { get; set; }

    public string PlantaAfectadaCode { get; set; } = default!;
    public Planta PlantaAfectada { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
