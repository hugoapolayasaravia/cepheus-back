using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioOrdenesEquipo.GetReporteCalendarioOrdenesEquipo;

public sealed record ReporteCalendarioOrdenesEquipoResponse(
    DateTime FechaDesde,
    DateTime FechaHasta,
    // Todos los días del rango, incluidos domingos (el legacy devolvía una fila por día aunque no hubiera OT).
    IReadOnlyList<DateTime> Dias,
    int TotalOrdenes,
    IReadOnlyList<ReporteCalendarioOrdenEquipoFilaResponse> Ordenes);

public sealed record ReporteCalendarioOrdenEquipoFilaResponse(
    string PlantaCode,
    string OrdenTrabajoCode,
    DateTime FechaOrden,
    string EquipoCode,
    string ActividadCode,
    // Gama: verbo + objeto de la actividad (legacy Descripcion_Ver + ' ' + Descripcion_Sus).
    string Gama,
    EstadoOrdenTrabajo Estado);
