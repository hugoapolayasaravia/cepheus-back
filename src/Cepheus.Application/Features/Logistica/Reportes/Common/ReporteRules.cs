using System.Text.RegularExpressions;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Application.Features.Logistica.Reportes.Common;

/// <summary>
/// Reglas compartidas por los reportes de Logística (traducidas de los SP legado Logi_sp_Rpt_*).
/// En los reportes un filtro "T" (todos) del legacy equivale a no enviar el parámetro.
/// </summary>
public static class ReporteRules
{
    /// <summary>Codigo_Est = '13' del legacy: solo documentos procesados.</summary>
    public const int EstadoProcesado = 13;

    /// <summary>Comprobante de pago Nota de Crédito: sus importes se muestran en negativo.</summary>
    public const string ComprobanteNotaCredito = "07";

    public const string MonedaDolar = "USD";

    /// <summary>
    /// Trabajador "P0001" de la salida por Orden de Servicio: el legacy invertía el signo del total
    /// (CASE R.Codigo_Tra WHEN 'P0001' THEN Total_Art * -1).
    /// </summary>
    public const string TrabajadorDevolucionOs = "P0001";

    public static bool EsTodos(string? valor)
        => string.IsNullOrWhiteSpace(valor) || valor.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);

    public static string Normalizar(string valor) => valor.Trim().ToUpperInvariant();

    /// <summary>Código normalizado, o null si el filtro es "todos".</summary>
    public static string? Filtro(string? valor) => EsTodos(valor) ? null : Normalizar(valor!);

    /// <summary>Rango [desde, hasta) para incluir todo el día de FechaHasta (el legacy usaba 23:59:59).</summary>
    public static (DateTime Desde, DateTime HastaExclusivo) Rango(DateTime desde, DateTime hasta)
        => (desde.Date, hasta.Date.AddDays(1));

    public static decimal Redondear(decimal valor, int decimales = 2)
        => Math.Round(valor, decimales, MidpointRounding.AwayFromZero);

    /// <summary>Convierte a soles si el documento está en dólares (importe * tipo de cambio), redondeado a 2 decimales.</summary>
    public static decimal ASoles(decimal importe, bool esDolar, decimal tipoCambio)
        => Redondear(esDolar ? importe * tipoCambio : importe);

    public static bool EsDolar(string monedaCode) => monedaCode == MonedaDolar;

    public static string NombreTrabajador(Trabajador t)
        => string.Join(" ", new[] { t.PaternalSurname, t.MaternalSurname, t.FirstNames }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

    /// <summary>Descripción legible de un estado (reemplaza TEstados.descripcion_est): "EntregaParcial" -> "Entrega Parcial".</summary>
    public static string Descripcion<TEnum>(TEnum valor) where TEnum : struct, Enum
        => Regex.Replace(valor.ToString(), "(?<=[a-z])(?=[A-Z])", " ");

    public static string Descripcion<TEnum>(TEnum? valor) where TEnum : struct, Enum
        => valor.HasValue ? Descripcion(valor.Value) : string.Empty;

    /// <summary>Normaliza el criterio de ordenamiento: reemplaza el ORDER BY dinámico (inyectable) del legacy por una lista blanca.</summary>
    public static string? OrdenSeguro(string? ordenarPor, IReadOnlyCollection<string> permitidos)
        => string.IsNullOrWhiteSpace(ordenarPor)
            ? null
            : permitidos.FirstOrDefault(p => p.Equals(ordenarPor.Trim(), StringComparison.OrdinalIgnoreCase));
}
