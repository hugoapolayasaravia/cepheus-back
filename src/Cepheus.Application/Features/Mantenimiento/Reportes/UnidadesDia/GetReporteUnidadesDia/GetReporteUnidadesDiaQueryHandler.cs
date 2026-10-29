using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Reportes.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.UnidadesDia.GetReporteUnidadesDia;

/// <summary>Solo lectura y sin SQL dinámico: reemplaza Logi_sp_Listado_SCcosto_PlantaAfectada.</summary>
public sealed class GetReporteUnidadesDiaQueryHandler
    : IRequestHandler<GetReporteUnidadesDiaQuery, ReporteUnidadesDiaResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteUnidadesDiaQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteUnidadesDiaResponse> Handle(
        GetReporteUnidadesDiaQuery request, CancellationToken ct)
    {
        var fecha = request.Fecha.Date;
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

        var horometros = (await _uow.Mantenimiento.Transacciones.UnidadesHorometro.Query().AsNoTracking()
                .Where(h => h.Fecha == fecha && codigos.Contains(h.SubCentroCostoCode))
                .Select(h => new
                {
                    h.SubCentroCostoCode,
                    h.PlantaAfectadaCode,
                    h.Estado,
                    h.Horometro,
                    h.EquipoCode,
                    EquipoNombre = h.Equipo != null ? h.Equipo.Name : null
                })
                .ToListAsync(ct))
            .ToDictionary(h => h.SubCentroCostoCode);

        var paradas = (await _uow.Mantenimiento.Transacciones.UnidadesParada.Query().AsNoTracking()
                .Where(p => p.Fecha == fecha && codigos.Contains(p.SubCentroCostoCode))
                .Select(p => new
                {
                    p.SubCentroCostoCode,
                    p.Horas,
                    p.FallaCode,
                    FallaNombre = p.Falla != null ? p.Falla.Name : null
                })
                .ToListAsync(ct))
            .ToDictionary(p => p.SubCentroCostoCode);

        var plantas = await _uow.Comunes.Plantas.Query().AsNoTracking()
            .Select(p => new { p.Code, p.Name })
            .ToDictionaryAsync(p => p.Code, p => p.Name, ct);

        var filas = unidades.Select(u =>
        {
            horometros.TryGetValue(u.Code, out var h);
            paradas.TryGetValue(u.Code, out var p);

            var plantaCode = h?.PlantaAfectadaCode ?? u.PlantaCode;

            return new ReporteUnidadDiaFilaResponse(
                u.Code, u.Name, u.IsActive,
                plantaCode,
                plantas.GetValueOrDefault(plantaCode),
                h?.Estado ?? EstadoEquipo.Operativo,
                h?.Horometro ?? 0m,
                h?.EquipoCode,
                h?.EquipoNombre,
                h is not null,
                p?.Horas,
                p?.FallaCode,
                p?.FallaNombre);
        }).ToList();

        return new ReporteUnidadesDiaResponse(fecha, filas.Count, filas);
    }
}
