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

/// <summary>Usuario que se identificó con login y clave en el cuadro «Aprobación por usuario».</summary>
public sealed record AprobadorIdentificado(
    int UserId,
    string Login,
    string NombreCompleto,
    string? TrabajadorCode,
    string? NivelCode,
    string? Rol);

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

    /// <summary>
    /// «Aprobación por usuario» del legacy: comprueba login y clave (usuario existente, clave correcta y activo).
    /// No cambia la sesión de quien llama. Lanza InvalidOperationException con el mensaje a mostrar.
    /// </summary>
    Task<AprobadorIdentificado> VerificarCredencialesAsync(string? login, string? clave, CancellationToken ct);

    /// <summary>
    /// Verifica las credenciales y además que ese usuario pueda aprobar el documento (nivel asignado,
    /// límite por documento y acumulados diario/mensual). No modifica nada.
    /// </summary>
    Task<AprobadorIdentificado> ValidarAprobadorAsync(DocumentoAprobable documento, string? login, string? clave, CancellationToken ct);
}
