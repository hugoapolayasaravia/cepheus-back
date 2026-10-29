using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Kardex: un registro por cada movimiento de stock de un artículo en una planta.
///
/// Reglas:
/// - Solo se INSERTA. Nunca se modifica ni se elimina (no hay Update/Delete). Una anulación
///   se registra como un movimiento nuevo de sentido contrario (EsReversion = true) enlazado
///   al original con ReversaDeMovimientoId.
/// - Lo escribe únicamente ArticuloStockMovementService, en la MISMA transacción que modifica
///   ArticuloStock: si el stock se guarda, el movimiento se guarda; si falla, ninguno.
/// - Id (identity) es el orden de llegada y desempata movimientos de la misma fecha.
///
/// Costos: son PROVISIONALES mientras el período está abierto (se calculan con el promedio vigente
/// al momento de registrar). El cierre mensual recalcula el costo promedio en orden cronológico y
/// deja los saldos valorizados en SaldoMensualArticulo.
///
/// Reemplaza a las tablas temporales globales MOVIMIENTOS / ERRORES del legacy, que se borraban
/// completas en cada ejecución y no eran seguras con varios usuarios.
/// </summary>
public class MovimientoArticulo : IAuditableEntity
{
    public long Id { get; set; }

    public string PlantaCode { get; set; } = default!;
    public Planta Planta { get; set; } = default!;

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

    /// <summary>Fecha del documento que mueve el stock (recepción / entrega), sin hora.</summary>
    public DateTime FechaMovimiento { get; set; }

    /// <summary>Período AAAAMM de FechaMovimiento. Se guarda para filtrar e indexar sin calcular.</summary>
    public string PeriodCode { get; set; } = default!;

    public TipoMovimientoArticulo Tipo { get; set; }
    public OrigenMovimientoArticulo Origen { get; set; }

    /// <summary>Código del documento de origen (nota de ingreso, orden de servicio, vale, ajuste...).</summary>
    public string DocumentoCode { get; set; } = default!;

    /// <summary>Ítem de la línea del documento (legacy Item_Art). Null en apertura y ajuste manual.</summary>
    public int? ItemNumber { get; set; }

    /// <summary>Siempre positiva; el sentido lo da Tipo.</summary>
    public decimal Cantidad { get; set; }

    /// <summary>Costo unitario NETO en soles (ya con descuento y tipo de cambio).</summary>
    public decimal CostoUnitario { get; set; }

    public decimal CostoUnitarioUsd { get; set; }

    /// <summary>Stock del artículo inmediatamente después de este movimiento.</summary>
    public decimal StockPosterior { get; set; }

    /// <summary>Costo promedio inmediatamente después de este movimiento (provisional).</summary>
    public decimal CostoPromedioPosterior { get; set; }

    /// <summary>True cuando el movimiento deshace a otro por anulación del documento.</summary>
    public bool EsReversion { get; set; }

    public long? ReversaDeMovimientoId { get; set; }
    public MovimientoArticulo? ReversaDeMovimiento { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
