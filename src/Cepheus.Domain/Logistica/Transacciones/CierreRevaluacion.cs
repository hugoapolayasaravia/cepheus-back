using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Auditoría de cada precio que el cierre corrigió en un documento de salida (vale, ajuste, salida de OS).
/// El legacy sobrescribía MValesDet.Precio_art sin dejar rastro; aquí queda el antes y el después, y permite
/// revertir la revalorización si se reabre el período.
/// </summary>
public class CierreRevaluacion : IAuditableEntity
{
    public long Id { get; set; }

    public string PlantaCode { get; set; } = default!;
    public string PeriodCode { get; set; } = default!;

    public OrigenMovimientoArticulo Origen { get; set; }
    public string DocumentoCode { get; set; } = default!;
    public int? ItemNumber { get; set; }
    public string ArticuloCode { get; set; } = default!;

    public decimal Cantidad { get; set; }
    public decimal PrecioAnterior { get; set; }
    public decimal PrecioNuevo { get; set; }
    public decimal TotalAnterior { get; set; }
    public decimal TotalNuevo { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
