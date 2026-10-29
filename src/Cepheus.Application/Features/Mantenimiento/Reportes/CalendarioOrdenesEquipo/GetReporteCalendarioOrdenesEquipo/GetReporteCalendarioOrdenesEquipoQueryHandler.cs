using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Reportes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioOrdenesEquipo.GetReporteCalendarioOrdenesEquipo;

/// <summary>
/// Solo lectura, sin SQL dinámico ni tablas temporales: reemplaza Man_sp_Listado_Calendario_Ot_Equipos.
/// En vez de unir con una tabla de fechas devuelve las OT y la lista de días del rango aparte.
/// </summary>
public sealed class GetReporteCalendarioOrdenesEquipoQueryHandler
    : IRequestHandler<GetReporteCalendarioOrdenesEquipoQuery, ReporteCalendarioOrdenesEquipoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteCalendarioOrdenesEquipoQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteCalendarioOrdenesEquipoResponse> Handle(
        GetReporteCalendarioOrdenesEquipoQuery request, CancellationToken ct)
    {
        var desde = request.FechaDesde.Date;
        var hastaExclusivo = request.FechaHasta.Date.AddDays(1);
        var planta = ReporteRules.Filtro(request.PlantaCode);
        var equipo = ReporteRules.Filtro(request.EquipoCode);

        var query = _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query().AsNoTracking()
            .Where(o => o.FechaProceso >= desde && o.FechaProceso < hastaExclusivo);

        if (planta is not null) query = query.Where(o => o.PlantaCode == planta);
        if (equipo is not null) query = query.Where(o => o.EquipoCode.Contains(equipo));
        if (request.Estado.HasValue) query = query.Where(o => o.Estado == request.Estado.Value);

        // Legacy: ORDER BY Codigo_Ori, Codigo_Otr
        var ordenes = await query
            .OrderBy(o => o.EquipoCode).ThenBy(o => o.Code)
            .Select(o => new ReporteCalendarioOrdenEquipoFilaResponse(
                o.PlantaCode,
                o.Code,
                o.FechaProceso,
                o.EquipoCode,
                o.ActividadCode,
                o.Actividad.VerboActividad.Name + " " + o.Actividad.ObjetoActividad.Name,
                o.Estado))
            .ToListAsync(ct);

        var dias = Enumerable.Range(0, (request.FechaHasta.Date - desde).Days + 1)
            .Select(i => desde.AddDays(i))
            .ToList();

        return new ReporteCalendarioOrdenesEquipoResponse(
            desde, request.FechaHasta.Date, dias, ordenes.Count, ordenes);
    }
}
