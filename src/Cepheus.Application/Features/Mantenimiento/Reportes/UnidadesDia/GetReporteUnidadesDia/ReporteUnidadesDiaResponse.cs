using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.UnidadesDia.GetReporteUnidadesDia;

public sealed record ReporteUnidadesDiaResponse(
    DateTime Fecha,
    int TotalUnidades,
    IReadOnlyList<ReporteUnidadDiaFilaResponse> Filas);

public sealed record ReporteUnidadDiaFilaResponse(
    string SubCentroCostoCode,
    string SubCentroCostoNombre,
    // Estado_Scc del legacy (subcentro habilitado).
    bool Activo,
    // Planta afectada del registro del día; si no hay registro, la planta del subcentro.
    string PlantaCode,
    string? PlantaNombre,
    // Estado del registro del día; si no hay registro, Operativo (legacy '22').
    EstadoEquipo Estado,
    decimal Horometro,
    string? EquipoCode,
    string? EquipoNombre,
    // true si ya existe lectura de horómetro para el día.
    bool TieneRegistro,
    // Parada no programada del mismo día (UnidadParada), si existe.
    decimal? HorasParada,
    string? FallaCode,
    string? FallaNombre);
