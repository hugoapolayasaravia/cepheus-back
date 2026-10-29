using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesPreventivas.GenerateOrdenesPreventivas;

/// <summary>
/// Valores fijos con que el botón "Genera OT" del legacy (ue_set_btn_grb_generar_ot) crea las OT preventivas
/// y sus vales de salida. Se centralizan para poder cambiarlos en un solo lugar.
/// </summary>
internal static class OrdenPreventivaRules
{
    // Catálogos de la OT (legacy: @Codigo_OPO, @Codigo_Pri, @Codigo_Ins, @Codigo_TOT).
    public const string OportunidadCode = "O1";
    public const string PrioridadCode = "P4";
    public const string InspeccionCode = "I5";
    public const string TipoOrdenCode = "MPV";

    // Legacy: la OT preventiva nace en EJECUCION (en proceso), no Pendiente.
    public const EstadoOrdenTrabajo EstadoInicial = EstadoOrdenTrabajo.EnProceso;

    // El legacy no envía turno; se usa el primero del catálogo.
    public const Turno TurnoPorDefecto = Turno.Manana;

    // Vale de salida generado con la OT (legacy: codigo_tva 'COT' = tipo "002"; Codigo_Une '001012').
    public const string TipoValeCode = "002";
    public const string UnidadNegocioCode = "001012";

    public const int OtCodeLength = 6;
    public const int OtDescriptionMaxLength = 254;
}
