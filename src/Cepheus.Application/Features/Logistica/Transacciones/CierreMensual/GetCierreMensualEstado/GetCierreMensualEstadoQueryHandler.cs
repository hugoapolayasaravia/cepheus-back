using System.Globalization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GetCierreMensualEstado;

public sealed class GetCierreMensualEstadoQueryHandler
    : IRequestHandler<GetCierreMensualEstadoQuery, CierreMensualEstadoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetCierreMensualEstadoQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<CierreMensualEstadoResponse> Handle(GetCierreMensualEstadoQuery request, CancellationToken ct)
    {
        var planta = request.PlantaCode.Trim().ToUpperInvariant();

        var plantaInfo = await _uow.Comunes.Plantas.Query().AsNoTracking()
            .Where(p => p.Code == planta)
            .Select(p => new { p.Name })
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"La planta {planta} no existe.");

        var ultimo = await _uow.Logistica.Maestros.ControlCierres.Query().AsNoTracking()
            .Where(c => c.PlantaCode == planta)
            .OrderByDescending(c => c.PeriodCode)
            .Select(c => new { c.PeriodCode, c.ClosureDate, c.DifferenceAmount })
            .FirstOrDefaultAsync(ct);

        if (ultimo is null)
        {
            var sugerido = await _uow.Logistica.Transacciones.MovimientosArticulo.Query().AsNoTracking()
                .Where(m => m.PlantaCode == planta)
                .OrderBy(m => m.PeriodCode)
                .Select(m => m.PeriodCode)
                .FirstOrDefaultAsync(ct);

            return new CierreMensualEstadoResponse(
                planta, plantaInfo.Name, null, null, null, sugerido,
                RequierePeriodoApertura: true,
                PuedeProcesar: sugerido is not null,
                Motivo: sugerido is null
                    ? "La planta no tiene movimientos de artículos registrados."
                    : "Es el primer cierre: confirme el período de apertura.",
                0, 0m);
        }

        var proximo = ControlCierreRules.NextPeriod(ultimo.PeriodCode);
        var fin = DateTime.ParseExact(proximo + "01", "yyyyMMdd", CultureInfo.InvariantCulture)
            .AddMonths(1).AddDays(-1);
        var terminado = fin < DateTime.Today;

        var incidencias = await _uow.Logistica.Transacciones.CierreIncidencias.Query().AsNoTracking()
            .CountAsync(i => i.PlantaCode == planta && i.PeriodCode == ultimo.PeriodCode, ct);

        return new CierreMensualEstadoResponse(
            planta, plantaInfo.Name,
            ultimo.PeriodCode, ultimo.ClosureDate,
            proximo, null,
            RequierePeriodoApertura: false,
            PuedeProcesar: terminado,
            Motivo: terminado ? null : $"El período {proximo} aún no termina (finaliza el {fin:dd/MM/yyyy}).",
            incidencias, ultimo.DifferenceAmount);
    }
}
