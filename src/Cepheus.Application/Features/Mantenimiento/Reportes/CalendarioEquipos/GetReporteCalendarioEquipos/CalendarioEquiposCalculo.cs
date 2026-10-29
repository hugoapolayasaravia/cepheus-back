using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Reportes.Common;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioEquipos.GetReporteCalendarioEquipos;

internal sealed record CalendarioCalculado(IReadOnlyList<DateTime> Dias, IReadOnlyList<GamaCalculada> Gamas);

/// <summary>
/// Carga los datos del calendario de equipos y ejecuta el cálculo. Lo comparten el reporte
/// (GetReporteCalendarioEquipos) y la generación de OT preventivas, para que ambos vean exactamente lo mismo.
/// </summary>
internal static class CalendarioEquiposCalculo
{
    public static async Task<CalendarioCalculado> CalcularAsync(
        IUnitOfWork uow,
        string? plantaCode,
        string? equipoCode,
        DateTime fechaDesde,
        DateTime fechaHasta,
        CancellationToken ct)
    {
        var desde = fechaDesde.Date;
        var hasta = fechaHasta.Date;
        var hastaExclusivo = hasta.AddDays(1);
        var planta = ReporteRules.Filtro(plantaCode);
        var equipo = ReporteRules.Filtro(equipoCode);

        var dias = CalendarioEquiposCalculator.DiasHabiles(desde, hasta);

        var planesQuery = uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query().AsNoTracking();
        if (planta is not null) planesQuery = planesQuery.Where(p => p.PlantaCode == planta);
        if (equipo is not null) planesQuery = planesQuery.Where(p => p.EquipoCode.Contains(equipo));

        var planes = await planesQuery
            .Select(p => new PlanCalendario(
                p.PlantaCode,
                p.Code,
                p.EquipoCode,
                p.ActividadCode,
                p.Actividad.VerboActividad.Name + " " + p.Actividad.ObjetoActividad.Name,
                p.Frecuencia,
                p.UnidadFrecuencia,
                p.FrecuenciaGama,
                p.TiempoRestante,
                p.SubCentroCostoCode))
            .ToListAsync(ct);

        if (planes.Count == 0)
        {
            return new CalendarioCalculado(dias, Array.Empty<GamaCalculada>());
        }

        var actividades = planes.Select(p => p.ActividadCode).Distinct().ToList();
        var plantas = planes.Select(p => p.PlantaCode).Distinct().ToList();
        var subCentros = planes.Select(p => p.SubCentroCostoCode).Distinct().ToList();

        var reglas = await uow.Mantenimiento.Maestros.ActividadesMantenimiento.Query().AsNoTracking()
            .Where(a => actividades.Contains(a.ActividadCode))
            .Select(a => new { a.ActividadCode, a.ActividadCubiertaCode, a.Prioridad })
            .ToListAsync(ct);

        // Legacy: la prioridad sale del JOIN con TActividadMantenimiento; si hay varias filas se procesa la menor.
        var prioridades = reglas
            .GroupBy(r => r.ActividadCode)
            .ToDictionary(g => g.Key, g => g.Min(r => r.Prioridad));
        var cubiertas = reglas.ToLookup(r => r.ActividadCode, r => r.ActividadCubiertaCode);

        var ordenesDb = await uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query().AsNoTracking()
            .Where(o => o.PlanMantenimientoPreventivoCode != null
                        && plantas.Contains(o.PlantaCode)
                        && o.FechaProceso >= desde && o.FechaProceso < hastaExclusivo)
            .Select(o => new { o.PlantaCode, Plan = o.PlanMantenimientoPreventivoCode!, o.FechaProceso })
            .ToListAsync(ct);
        var ordenes = ordenesDb
            .Select(o => (o.PlantaCode, o.Plan, o.FechaProceso.Date))
            .ToHashSet();

        var paradasDb = await uow.Mantenimiento.Transacciones.UnidadesParada.Query().AsNoTracking()
            .Where(p => subCentros.Contains(p.SubCentroCostoCode) && p.Fecha >= desde && p.Fecha <= hasta)
            .Select(p => new { p.SubCentroCostoCode, p.Fecha })
            .ToListAsync(ct);
        var paradas = paradasDb
            .Select(p => (p.SubCentroCostoCode, p.Fecha.Date))
            .ToHashSet();

        var gamas = CalendarioEquiposCalculator.Calcular(desde, hasta, planes, prioridades, cubiertas, ordenes, paradas);

        return new CalendarioCalculado(dias, gamas);
    }
}
