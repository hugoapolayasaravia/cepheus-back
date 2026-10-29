namespace Cepheus.Application.Features.Logistica.Reportes.Common;

/// <summary>
/// Fecha por la que se filtra el consumo (@opt del legacy). E = entrega, S = proceso, A = aprobación.
/// La opción P (Fecha_Prs) no existe: el Vale solo guarda el indicador Preparado, sin fecha.
/// </summary>
public static class TipoFechaConsumo
{
    public const string Entrega = "E";
    public const string Proceso = "S";
    public const string Aprobacion = "A";

    public static string Normalizar(string? valor)
        => string.IsNullOrWhiteSpace(valor) ? Entrega : valor.Trim().ToUpperInvariant();

    public static bool EsValido(string? valor)
        => Normalizar(valor) is Entrega or Proceso or Aprobacion;

    public static DateTime? Elegir(string tipo, DateTime entrega, DateTime? proceso, DateTime? aprobacion)
        => tipo switch
        {
            Proceso => proceso,
            Aprobacion => aprobacion,
            _ => entrega
        };
}
