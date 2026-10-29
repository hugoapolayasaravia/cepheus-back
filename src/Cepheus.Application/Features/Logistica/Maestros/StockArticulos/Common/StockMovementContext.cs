using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;

/// <summary>
/// Datos del documento que origina un movimiento de stock. Con esto el servicio central registra el
/// movimiento en el Kardex y valida que el período no esté cerrado.
/// </summary>
/// <param name="Origen">Documento o proceso que mueve el stock.</param>
/// <param name="DocumentoCode">
/// Código del documento. Puede ser null SOLO cuando el documento aún no tiene correlativo al mover el stock
/// (p. ej. Nota de Ingreso nueva): quien llama debe asignarlo en MovimientoArticulo.DocumentoCode antes de
/// SaveChanges; si no lo hace, el guardado falla (la columna es obligatoria).
/// </param>
/// <param name="ItemNumber">Ítem de la línea del documento; null si no aplica.</param>
/// <param name="FechaMovimiento">Fecha del documento (recepción / entrega). Define el período del movimiento.</param>
/// <param name="CostoReferencia">
/// Costo unitario neto en soles a registrar en el Kardex cuando el movimiento no recalcula el promedio
/// (devoluciones). Si es null: en las salidas se usa el costo promedio vigente.
/// </param>
/// <param name="EsReversion">True cuando el movimiento deshace a otro por anulación del documento.</param>
public sealed record StockMovementContext(
    OrigenMovimientoArticulo Origen,
    string? DocumentoCode,
    int? ItemNumber,
    DateTime FechaMovimiento,
    decimal? CostoReferencia = null,
    bool EsReversion = false);

/// <summary>Resultado de un movimiento: el stock afectado y la fila del Kardex que quedó registrada.</summary>
public sealed record StockMovementResult(ArticuloStock Stock, MovimientoArticulo Movimiento);
