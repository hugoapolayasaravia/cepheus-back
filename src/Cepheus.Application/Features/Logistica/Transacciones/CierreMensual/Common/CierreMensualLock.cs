using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;

/// <summary>
/// Serializa los procesos de cierre de una misma planta usando una fila de bloqueo (CierreBloqueos) y un UPDATE normal:
/// el segundo proceso espera a que el primero confirme o revierta. No usa sp_getapplock ni advisory locks, así que
/// funciona igual en SQL Server y en PostgreSQL.
/// </summary>
public static class CierreMensualLock
{
    private static readonly TimeSpan Espera = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Garantiza que exista la fila de bloqueo de la planta. Debe llamarse ANTES de abrir la transacción del proceso
    /// (si dos usuarios la crean a la vez, uno falla por clave duplicada y se ignora).
    /// </summary>
    public static async Task EnsureAsync(IUnitOfWork uow, string plantaCode, CancellationToken ct)
    {
        var existe = await uow.Logistica.Transacciones.CierreBloqueos.Query().AsNoTracking()
            .AnyAsync(b => b.PlantaCode == plantaCode, ct);

        if (existe)
            return;

        try
        {
            await uow.Logistica.Transacciones.CierreBloqueos.AddAsync(
                new CierreBloqueo { PlantaCode = plantaCode, UltimoUso = DateTime.UtcNow }, ct);
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Otro usuario creó la fila al mismo tiempo: es lo que se buscaba.
            uow.ClearTracking();
        }
    }

    /// <summary>Dentro de la transacción: toma el bloqueo de la planta hasta confirmar o revertir.</summary>
    public static async Task AcquireAsync(IUnitOfWork uow, string plantaCode, CancellationToken ct)
    {
        using var limite = CancellationTokenSource.CreateLinkedTokenSource(ct);
        limite.CancelAfter(Espera);

        try
        {
            var fila = await uow.Logistica.Transacciones.CierreBloqueos.Query()
                .FirstOrDefaultAsync(b => b.PlantaCode == plantaCode, limite.Token)
                ?? throw new InvalidOperationException(
                    $"No existe la fila de bloqueo de cierre de la planta {plantaCode}.");

            fila.UltimoUso = DateTime.UtcNow;

            // El UPDATE toma el bloqueo de la fila; si otro proceso lo tiene, espera hasta el límite.
            await uow.SaveChangesAsync(limite.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new InvalidOperationException(
                "Otro usuario está ejecutando este proceso para la misma planta. Intente nuevamente en unos minutos.");
        }
    }
}
