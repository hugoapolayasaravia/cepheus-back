using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioEquipos.GetReporteCalendarioEquipos;

/// <summary>Solo lectura: reemplaza Man_sp_Listado_Calendario_Equipos. El cálculo vive en CalendarioEquiposCalculo.</summary>
public sealed class GetReporteCalendarioEquiposQueryHandler
    : IRequestHandler<GetReporteCalendarioEquiposQuery, ReporteCalendarioEquiposResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteCalendarioEquiposQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteCalendarioEquiposResponse> Handle(
        GetReporteCalendarioEquiposQuery request, CancellationToken ct)
    {
        var desde = request.FechaDesde.Date;
        var hasta = request.FechaHasta.Date;

        var calculo = await CalendarioEquiposCalculo.CalcularAsync(
            _uow, request.PlantaCode, request.EquipoCode, desde, hasta, ct);

        var gamas = calculo.Gamas
            .Select(g => new ReporteCalendarioEquiposGamaResponse(
                g.Plan.PlantaCode,
                g.Plan.PlanCode,
                g.Plan.EquipoCode,
                g.Plan.ActividadCode,
                g.Plan.Gama,
                g.Plan.Frecuencia,
                g.Plan.Unidad,
                g.Plan.FrecuenciaGama,
                g.Plan.TiempoRestante,
                g.Prioridad,
                g.Celdas.Select(c => new ReporteCalendarioEquiposCeldaResponse(
                    c.Fecha, c.Dato, c.HayOrden, CalendarioEquiposCalculator.TocaGenerarOrden(c.Dato))).ToList()))
            .ToList();

        return new ReporteCalendarioEquiposResponse(
            desde, hasta,
            calculo.Dias.Select((d, i) => new ReporteCalendarioEquiposDiaResponse(i + 1, d)).ToList(),
            gamas.Count, gamas);
    }
}
