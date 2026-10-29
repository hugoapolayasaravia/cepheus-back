using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.HorometroCalendario.GetReporteHorometroCalendario;

public sealed record ReporteHorometroCalendarioResponse(
    DateTime FechaDesde,
    DateTime FechaHasta,
    // Todos los días del rango (el legacy devolvía una fila por día aunque no hubiera datos).
    IReadOnlyList<DateTime> Dias,
    int TotalUnidades,
    IReadOnlyList<ReporteHorometroCalendarioUnidadResponse> Unidades);

public sealed record ReporteHorometroCalendarioUnidadResponse(
    string SubCentroCostoCode,
    string SubCentroCostoNombre,
    // Estado_Scc del legacy (subcentro habilitado).
    bool Activo,
    string PlantaCode,
    // Solo los días con lectura; los días sin lectura no aparecen (se completan con Dias).
    IReadOnlyList<ReporteHorometroCalendarioLecturaResponse> Lecturas);

public sealed record ReporteHorometroCalendarioLecturaResponse(
    DateTime Fecha,
    decimal Horometro,
    EstadoEquipo Estado,
    string PlantaAfectadaCode);
