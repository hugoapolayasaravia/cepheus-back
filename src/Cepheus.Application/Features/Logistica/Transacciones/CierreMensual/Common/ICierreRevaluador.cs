using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;

/// <summary>Salida de un documento que debe quedar valorizada a PrecioNuevo (soles).</summary>
public sealed record SalidaValorizada(
    OrigenMovimientoArticulo Origen,
    string DocumentoCode,
    int? ItemNumber,
    string ArticuloCode,
    decimal Cantidad,
    decimal PrecioNuevo);

/// <summary>Precio corregido en un documento (queda en CierreRevaluaciones como auditoría).</summary>
public sealed record RevaluacionAplicada(
    OrigenMovimientoArticulo Origen,
    string DocumentoCode,
    int? ItemNumber,
    string ArticuloCode,
    decimal Cantidad,
    decimal PrecioAnterior,
    decimal PrecioNuevo,
    decimal TotalAnterior,
    decimal TotalNuevo);

/// <summary>
/// Corrige los precios de los documentos de salida de un tipo de origen cuando el cierre determina el costo real
/// (en el legacy: Consumos y Consumos_nc actualizaban MValesDet / MAjustesInventarioDet / MOrdenServicioDetS y
/// recalculaban neto, IGV y total). Hay un revaluador por tipo de documento:
///   - OrdenServicio -> OrdenServicioSalidaRevaluador (incluido)
///   - ValeSalida    -> pendiente: implementarlo cuando el módulo de Vales esté en el repo
///   - AjusteSalida  -> pendiente: implementarlo cuando el módulo de Ajustes esté en el repo
/// Se registran en DI como ICierreRevaluador y el cierre los toma todos. No ejecutan SaveChanges: el cierre
/// confirma todo en una sola transacción.
/// </summary>
public interface ICierreRevaluador
{
    IReadOnlyCollection<OrigenMovimientoArticulo> Origenes { get; }

    Task<IReadOnlyList<RevaluacionAplicada>> AplicarAsync(
        string plantaCode,
        string periodCode,
        IReadOnlyList<SalidaValorizada> salidas,
        CancellationToken cancellationToken);

    /// <summary>Restaura los precios anteriores (reapertura del período).</summary>
    Task RevertirAsync(
        string plantaCode,
        string periodCode,
        IReadOnlyList<CierreRevaluacion> auditoria,
        CancellationToken cancellationToken);
}
