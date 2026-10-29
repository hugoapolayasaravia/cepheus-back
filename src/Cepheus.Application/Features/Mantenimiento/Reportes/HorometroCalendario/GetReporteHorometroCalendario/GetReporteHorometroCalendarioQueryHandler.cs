using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Reportes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.HorometroCalendario.GetReporteHorometroCalendario;

/// <summary>
/// Solo lectura y sin SQL dinámico ni tabla temporal: reemplaza Man_sp_Listado_Equipos_Horometro_Calendario.
/// En vez de filas planas (unidad x día) agrupa por unidad y devuelve la lista de días del rango aparte.
/// </summary>
public sealed class GetReporteHorometroCalendarioQueryHandler
    : IRequestHandler<GetReporteHorometroCalendarioQuery, ReporteHorometroCalendarioResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteHorometroCalendarioQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteHorometroCalendarioResponse> Handle(
        GetReporteHorometroCalendarioQuery request, CancellationToken ct)
    {
        var desde = request.FechaDesde.Date;
        var hasta = request.FechaHasta.Date;
        var cc = ReporteRules.Filtro(request.CentroCostoCode);
        var planta = ReporteRules.Filtro(request.PlantaCode);

        // Legacy: INNER JOIN TMCentroCosto (solo subcentros con centro de costo) y Unidad_SN = 'S'.
        var unidadesQuery = _uow.Logistica.Maestros.SubCentrosCosto.Query().AsNoTracking()
            .Where(s => s.IsUnit && s.CentroCostoCode != null);

        if (cc is not null) unidadesQuery = unidadesQuery.Where(s => s.CentroCostoCode == cc);
        if (planta is not null) unidadesQuery = unidadesQuery.Where(s => s.PlantaCode == planta);

        var unidades = await unidadesQuery
            .OrderBy(s => s.Code)
            .Select(s => new { s.Code, s.Name, s.IsActive, s.PlantaCode })
            .ToListAsync(ct);

        var codigos = unidades.Select(u => u.Code).ToList();

        var lecturas = (await _uow.Mantenimiento.Transacciones.UnidadesHorometro.Query().AsNoTracking()
                .Where(h => h.Fecha >= desde && h.Fecha <= hasta && codigos.Contains(h.SubCentroCostoCode))
                .OrderBy(h => h.Fecha)
                .Select(h => new ReporteHorometroCalendarioLecturaConUnidad(
                    h.SubCentroCostoCode, h.Fecha, h.Horometro, h.Estado, h.PlantaAfectadaCode))
                .ToListAsync(ct))
            .ToLookup(l => l.SubCentroCostoCode);

        var dias = Enumerable.Range(0, (hasta - desde).Days + 1)
            .Select(i => desde.AddDays(i))
            .ToList();

        var filas = unidades.Select(u => new ReporteHorometroCalendarioUnidadResponse(
            u.Code, u.Name, u.IsActive, u.PlantaCode,
            lecturas[u.Code]
                .Select(l => new ReporteHorometroCalendarioLecturaResponse(
                    l.Fecha, l.Horometro, l.Estado, l.PlantaAfectadaCode))
                .ToList())).ToList();

        return new ReporteHorometroCalendarioResponse(desde, hasta, dias, filas.Count, filas);
    }

    private sealed record ReporteHorometroCalendarioLecturaConUnidad(
        string SubCentroCostoCode,
        DateTime Fecha,
        decimal Horometro,
        Cepheus.Domain.Mantenimiento.Enum.EstadoEquipo Estado,
        string PlantaAfectadaCode);
}
