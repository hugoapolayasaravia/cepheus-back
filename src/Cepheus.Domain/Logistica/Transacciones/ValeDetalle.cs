using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Línea de Vale de Salida.
///
/// Legacy: dbo.MValesDet. PK legacy y de esta entidad: (Codigo_Pla, Codigo_Val, Codigo_Art).
///   Codigo_Art   -> ArticuloCode
///   Item_Art     -> ItemNumber (correlativo 1..n; se renumera por código de artículo al eliminar)
///   Cantidad_Art -> Cantidad
///   Precio_Art   -> Precio (precio promedio vigente del artículo en la planta; lo resuelve el servidor)
///   Total_Art    -> Total (Cantidad × Precio, redondeado a 2 decimales)
///   Codigo_Est   -> Estado
///   Asiento_val  -> AsientoContable (dato de contabilidad, se mantiene)
///   Propiedad01  -> Propiedad01 (horómetro; obligatorio si Articulo.RequiresHorometro)
///
/// MaterialOtFechaProceso: si la línea nació de un material pendiente de la Orden de Trabajo del vale,
/// guarda la FechaProceso de ese OTRMaterial (la llave es Planta + OT + FechaProceso + Artículo).
/// Permite marcarlo asignado (15) al crear, consumido (13) al procesar y liberarlo al borrar / devolver / anular.
/// </summary>
public class ValeDetalle : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public string ValeCode { get; set; } = default!;
    public Vale Vale { get; set; } = default!;

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

    public int ItemNumber { get; set; }

    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Total { get; set; }

    public EstadoValeDetalle Estado { get; set; } = EstadoValeDetalle.Pendiente;

    public string? AsientoContable { get; set; }
    public string? Propiedad01 { get; set; }

    public DateTime? MaterialOtFechaProceso { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
