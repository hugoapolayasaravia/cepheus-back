using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Ajuste de Inventario (cabecera): corrige el stock de un almacén sin pasar por compra ni vale
/// (sobrantes suman, faltantes restan).
///
/// Legacy: dbo.MAjustesInventario (SQL Server). Mapeo de columnas legacy -> propiedades:
///   Codigo_Pla       -> PlantaCode (almacén; FK Comunes.Planta)
///   Codigo_Aju       -> Code (correlativo por planta, 6 dígitos)
///   Observacion_Aju  -> Observacion
///   Fecha_ent        -> FechaEntrega
///   Neto_Aju         -> Neto
///   Igv_Aju          -> Igv
///   Monto_Aju        -> Total
///   Fecha_Pro        -> CreatedAt (auditoría)
///   Codigo_Est       -> Estado (enum, ver EstadoAjusteInventario)
///   Usuario          -> CreatedBy (auditoría)
///   Asiento_Aju      -> AsientoContable (dato de contabilidad, se mantiene)
/// </summary>
public class AjusteInventario : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public Planta Planta { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string? Observacion { get; set; }

    public DateTime FechaEntrega { get; set; }

    public decimal Neto { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }

    public EstadoAjusteInventario Estado { get; set; } = EstadoAjusteInventario.Pendiente;

    public string? AsientoContable { get; set; }

    public ICollection<AjusteInventarioDetalle> Detalles { get; set; } = new List<AjusteInventarioDetalle>();

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
