namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;

/// <summary>
/// Códigos de TipoTransaccion que participan de la bandeja de aprobación
/// (legacy: w_log_aprobacion_tra, columna "tras" de Parametros_Listado).
/// Los códigos viven en la tabla TipoTransaccion; acá solo se nombran los que el código ya usa
/// (OC, OS, VS, VC) más SC para el pedido.
/// </summary>
public static class AprobacionTipos
{
    public const string Pedido = "SC";
    public const string OrdenCompra = "OC";
    public const string OrdenServicio = "OS";
    public const string Vale = "VS";
    public const string ValeCliente = "VC";

    public const string MonedaSoles = "PEN";

    public static readonly string[] Todos = { Pedido, OrdenCompra, OrdenServicio, Vale, ValeCliente };

    public static string Normalizar(string? tipo) => (tipo ?? string.Empty).Trim().ToUpperInvariant();
}
