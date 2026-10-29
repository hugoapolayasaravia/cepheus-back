using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes;

/// <summary>
/// Porcentaje de IGV vigente en un rango de fechas. Reemplaza el uso de ControlVentas.IgvPercentage (un único valor
/// "actual") en el módulo de Logística: cada documento toma el porcentaje del período que contiene su fecha, como hacía
/// el legacy con fg_igv(fecha).
///
/// Reglas:
///   - Los períodos no se solapan (se valida al crear y al modificar).
///   - FechaFin nula = vigente hasta nuevo aviso (el período actual).
///   - Si una fecha no cae en ningún período, el cálculo falla con un mensaje claro: no se usa un valor por defecto,
///     para no aplicar en silencio una tasa equivocada a un documento antiguo.
///
/// Es un registro histórico (como TipoCambio): no tiene Code ni IsActive.
/// </summary>
public class PeriodoIgv : IAuditableEntity
{
    public int Id { get; set; }

    /// <summary>Primer día en que rige el porcentaje (inclusive).</summary>
    public DateOnly FechaInicio { get; set; }

    /// <summary>Último día en que rige el porcentaje (inclusive). Null = sin fecha de fin.</summary>
    public DateOnly? FechaFin { get; set; }

    /// <summary>Porcentaje de IGV (ej. 18 para 18%).</summary>
    public decimal PorcentajeIgv { get; set; }

    // Auditoría (IAuditableEntity)
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Concurrencia optimista
    public byte[] RowVersion { get; set; } = default!;
}
