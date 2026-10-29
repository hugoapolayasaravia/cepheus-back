using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Mantenimiento.Maestros;
using Cepheus.Domain.Mantenimiento.Transacciones;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Línea del vale de salida de una Orden de Servicio. Espejo de la línea de la orden (mismo ítem, artículo
/// y cantidad) pero con precio y total en SOLES.
///
/// Legacy: dbo.MOrdenServicioDetS, PK (Codigo_Pla, Codigo_Val, Codigo_Art, Item_Art). Se usa la PK del
/// resto de líneas del repositorio: (PlantaCode, SalidaCode, ItemNumber).
///   Codigo_Val    -> SalidaCode            Item_Art      -> ItemNumber
///   Codigo_Art    -> ArticuloCode          Glosa_Art     -> Glosa
///   Cantidad_Art  -> Cantidad              Precio_Art    -> Precio (soles)
///   Total_Art     -> Total (soles)         Codigo_Est    -> Estado (enum)
///   Asiento_val   -> AsientoContable       Propiedad01   -> Propiedad01
///   Codigo_tva    -> TipoValeCode          Codigo_Scc    -> SubCentroCostoCode
///   Codigo_Sce    -> SubCentroEjecutorCode Codigo_Otr    -> OrdenTrabajoCode
///   Cod_Planta    -> PlantaAfectadaCode
/// La sincronización con las líneas de la orden la hace OrdenServicioSalidaSync.
/// </summary>
public class OrdenServicioSalidaDetalle : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public string SalidaCode { get; set; } = default!;
    public OrdenServicioSalida Salida { get; set; } = default!;

    public int ItemNumber { get; set; }

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

    public string? Glosa { get; set; }

    public decimal Cantidad { get; set; }
    /// <summary>Precio unitario en soles.</summary>
    public decimal Precio { get; set; }
    /// <summary>Total de la línea en soles.</summary>
    public decimal Total { get; set; }

    public EstadoOrdenServicioSalidaDetalle Estado { get; set; } = EstadoOrdenServicioSalidaDetalle.Pendiente;

    public string? AsientoContable { get; set; }
    public string? Propiedad01 { get; set; }

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
