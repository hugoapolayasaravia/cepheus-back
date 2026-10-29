using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Línea de Ajuste de Inventario.
///
/// Legacy: dbo.MAjustesInventarioDet. PK legacy y de esta entidad: (Codigo_Pla, Codigo_Aju, Codigo_Art).
///   Codigo_Art   -> ArticuloCode
///   Item_Art     -> ItemNumber (correlativo 1..n; se renumera por código de artículo al eliminar)
///   Cantidad_Art -> Cantidad
///   Precio_Art   -> Precio (precio promedio vigente del artículo en la planta; no es editable, lo resuelve el servidor)
///   Total_Art    -> Total (Cantidad × Precio, redondeado a 2 decimales)
///   Codigo_Est   -> Estado
///   Tipo_Aju     -> Tipo (Sobrante I / Faltante S)
///   Asiento_Aju  -> AsientoContable (dato de contabilidad, se mantiene)
/// </summary>
public class AjusteInventarioDetalle : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public string AjusteCode { get; set; } = default!;
    public AjusteInventario Ajuste { get; set; } = default!;

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

    public int ItemNumber { get; set; }

    public TipoAjusteInventario Tipo { get; set; }

    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Total { get; set; }

    public EstadoAjusteInventarioDetalle Estado { get; set; } = EstadoAjusteInventarioDetalle.Pendiente;

    public string? AsientoContable { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
