namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;

/// <summary>Documento sobre el que se decide quién puede aprobar.</summary>
public sealed record DocumentoAprobable(
    string TipoTransaccion,
    string PlantaCode,
    string Code,
    string UnidadNegocioCode,
    string MonedaCode,
    decimal Monto);

/// <summary>Persona habilitada para aprobar un documento en este momento. Rol: TITULAR, SUPLENTE o SUPERIOR.</summary>
public sealed record AprobadorHabilitado(
    string NivelCode,
    string Rol,
    string TrabajadorCode);

public sealed record ResultadoHabilitados(
    IReadOnlyList<AprobadorHabilitado> Habilitados,
    IReadOnlyList<string> Observaciones);

/// <summary>
/// Regla de aprobación del legacy (Logi_sp_Validar_Aprobacion / Logi_sp_Verifica_Aprobacion_Transaccion),
/// calculada en el momento sobre RangoAprobacion + AprobadorAsignado (no existe Parametros_Listado):
///  - Se toma el nivel ASIGNADO al aprobador (no el rango exacto del monto).
///  - El titular (y su suplente) aprueba si monto &lt;= ImporteMaximo del nivel y, sumando lo que el titular ya
///    aprobó ese día / ese mes (por FechaAprobacion), no supera ImporteAcumuladoDiario / ImporteAcumuladoMensual.
///  - Si el titular no cubre el documento, lo aprueba el SUPERIOR asignado a esa fila.
///  - Los montos acumulados en otra moneda se convierten con TipoCambio.SellRate de la fecha de aprobación.
/// Sin excepciones por usuario ni por unidad (el código duro del legacy no se migra).
/// </summary>
public interface IAprobacionService
{
    /// <summary>Código de Trabajador vinculado al usuario autenticado (null si no tiene).</summary>
    Task<string?> ObtenerTrabajadorActualAsync(CancellationToken ct);

    /// <summary>Quiénes pueden aprobar el documento ahora mismo.</summary>
    Task<ResultadoHabilitados> ObtenerHabilitadosAsync(DocumentoAprobable documento, CancellationToken ct);

    /// <summary>Lanza InvalidOperationException si el usuario autenticado no puede aprobar el documento.</summary>
    Task AsegurarAprobadorActualAsync(DocumentoAprobable documento, CancellationToken ct);
}
