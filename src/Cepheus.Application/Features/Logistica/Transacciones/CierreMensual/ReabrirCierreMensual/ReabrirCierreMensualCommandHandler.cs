using System.Data;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.ReabrirCierreMensual;

/// <summary>
/// Deshace el último cierre: restaura los precios que el cierre había corregido en los documentos, elimina los saldos,
/// incidencias y auditoría del período y borra su ControlCierre. Todo en una transacción y bajo el mismo bloqueo por
/// planta que el cierre. Solo se puede reabrir el último período cerrado.
/// El costo promedio vivo del stock NO se revierte: queda con el valor que dejó el cierre hasta el próximo cierre.
/// </summary>
public sealed class ReabrirCierreMensualCommandHandler
    : IRequestHandler<ReabrirCierreMensualCommand, ReabrirCierreMensualResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IReadOnlyList<ICierreRevaluador> _revaluadores;

    public ReabrirCierreMensualCommandHandler(IUnitOfWork uow, IEnumerable<ICierreRevaluador> revaluadores)
    {
        _uow = uow;
        _revaluadores = revaluadores.ToList();
    }

    public async Task<ReabrirCierreMensualResponse> Handle(ReabrirCierreMensualCommand request, CancellationToken ct)
    {
        var planta = request.PlantaCode.Trim().ToUpperInvariant();

        await CierreMensualLock.EnsureAsync(_uow, planta, ct);

        try
        {
            return await _uow.ExecuteInTransactionAsync(
                token => EjecutarAsync(planta, token),
                IsolationLevel.ReadCommitted,
                ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "Un documento cambió mientras se reabría el período. Intente nuevamente.");
        }
    }

    private async Task<ReabrirCierreMensualResponse> EjecutarAsync(string planta, CancellationToken ct)
    {
        await CierreMensualLock.AcquireAsync(_uow, planta, ct);

        var ultimo = await _uow.Logistica.Maestros.ControlCierres.Query()
            .Where(c => c.PlantaCode == planta)
            .OrderByDescending(c => c.PeriodCode)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"La planta {planta} no tiene períodos cerrados.");

        var periodo = ultimo.PeriodCode;

        var auditoria = await _uow.Logistica.Transacciones.CierreRevaluaciones.Query()
            .AsNoTracking()
            .Where(r => r.PlantaCode == planta && r.PeriodCode == periodo)
            .ToListAsync(ct);

        foreach (var revaluador in _revaluadores)
        {
            await revaluador.RevertirAsync(planta, periodo, auditoria, ct);
        }

        // Confirma la restauración de los documentos antes de borrar la auditoría que la respalda.
        await _uow.SaveChangesAsync(ct);

        var documentos = auditoria.Select(a => (a.Origen, a.DocumentoCode)).Distinct().Count();

        await _uow.Logistica.Transacciones.CierreRevaluaciones.Query()
            .Where(r => r.PlantaCode == planta && r.PeriodCode == periodo)
            .ExecuteDeleteAsync(ct);

        await _uow.Logistica.Transacciones.CierreIncidencias.Query()
            .Where(i => i.PlantaCode == planta && i.PeriodCode == periodo)
            .ExecuteDeleteAsync(ct);

        var saldos = await _uow.Logistica.Transacciones.SaldosMensualesArticulo.Query()
            .Where(s => s.PlantaCode == planta && s.PeriodCode == periodo)
            .ExecuteDeleteAsync(ct);

        _uow.Logistica.Maestros.ControlCierres.Remove(ultimo);
        await _uow.SaveChangesAsync(ct);

        return new ReabrirCierreMensualResponse(planta, periodo, documentos, saldos);
    }
}
