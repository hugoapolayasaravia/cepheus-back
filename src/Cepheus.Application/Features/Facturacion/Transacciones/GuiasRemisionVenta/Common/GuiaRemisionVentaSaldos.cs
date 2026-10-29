using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;

/// <summary>
/// Equivalente de ad_actualizasaldos: recalcula lo entregado de las líneas de la OT y de la programación de la guía, el
/// estado de la programación y el de la OT. Se ejecuta antes de guardar: suma las guías ya guardadas (sin la actual) más
/// las líneas en memoria de la actual, así todo se confirma en un solo SaveChanges.
/// </summary>
public static class GuiaRemisionVentaSaldos
{
    private readonly record struct Row(
        int? Item,
        MotivoGuiaRemisionVenta Motive,
        string? Action,
        TipoProgramacionGuia? PType,
        string? PCode,
        decimal Qty);

    /// <summary>Entregado +: toda guía salvo devolución. Devolución con acción "01" resta; con otra acción no cuenta.</summary>
    private static decimal Signed(MotivoGuiaRemisionVenta motive, string? action, decimal qty)
        => motive == MotivoGuiaRemisionVenta.Devolucion
            ? (action == GuiaRemisionVentaRules.ActionReturnsToOrder ? -qty : 0m)
            : qty;

    /// <param name="ot">OT con Detalles, tracked.</param>
    /// <param name="current">Guía creada/modificada/anulada (tracked, con sus líneas en memoria).</param>
    public static Task RecalcAsync(
        IUnitOfWork uow,
        OrdenTrabajoVenta ot,
        GuiaRemisionVenta current,
        CancellationToken ct)
        => RecalcAsync(uow, ot, new[] { current }, ct);

    /// <summary>
    /// Igual que el anterior para varias guías a la vez (anulación o liberación de un comprobante con varias guías):
    /// ninguna de las indicadas se lee de la base; se usan sus líneas y estado en memoria.
    /// </summary>
    public static async Task RecalcAsync(
        IUnitOfWork uow,
        OrdenTrabajoVenta ot,
        IReadOnlyCollection<GuiaRemisionVenta> currents,
        CancellationToken ct)
    {
        var currentNumbers = currents.Select(c => c.Number).ToList();

        var stored = await uow.Facturacion.Transacciones.GuiasRemisionVentaDetalle
            .Query().AsNoTracking()
            .Where(d => d.OrdenTrabajoCode == ot.Code
                        && d.OrdenTrabajoItem != null
                        && !currentNumbers.Contains(d.GuiaNumber)
                        && d.Guia.Status != EstadoGuiaRemisionVenta.Anulado
                        && !d.Guia.IsBalance)
            .Select(d => new
            {
                d.OrdenTrabajoItem,
                d.Guia.Motive,
                d.Guia.DevolutionActionCode,
                d.Guia.ProgramacionType,
                d.Guia.ProgramacionCode,
                d.Quantity
            })
            .ToListAsync(ct);

        var rows = stored
            .Select(s => new Row(s.OrdenTrabajoItem, s.Motive, s.DevolutionActionCode, s.ProgramacionType, s.ProgramacionCode, s.Quantity))
            .ToList();

        foreach (var current in currents.Where(c => c.Status != EstadoGuiaRemisionVenta.Anulado && !c.IsBalance))
        {
            rows.AddRange(current.Detalles
                .Where(d => d.OrdenTrabajoItem != null && d.OrdenTrabajoCode == ot.Code)
                .Select(d => new Row(d.OrdenTrabajoItem, current.Motive, current.DevolutionActionCode,
                    current.ProgramacionType, current.ProgramacionCode, d.Quantity)));
        }

        // --- Líneas de la OT
        var byItem = rows
            .GroupBy(r => r.Item!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(r => Signed(r.Motive, r.Action, r.Qty)));

        foreach (var line in ot.Detalles)
        {
            line.DeliveredQuantity = Math.Max(0m, byItem.GetValueOrDefault(line.Item));
        }

        // --- Programaciones de las guías
        var progs = currents
            .Where(c => c.ProgramacionType != null && c.ProgramacionCode != null)
            .Select(c => (Type: c.ProgramacionType!.Value, Code: c.ProgramacionCode!))
            .Distinct()
            .ToList();

        foreach (var (pType, pCode) in progs)
        {
            var byProgItem = rows
                .Where(r => r.PType == pType && r.PCode == pCode && r.Motive != MotivoGuiaRemisionVenta.Devolucion)
                .GroupBy(r => r.Item!.Value)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Qty));

            await GuiaRemisionVentaProgramaciones.ApplyDeliveredAsync(uow, pType, pCode, byProgItem, ct);
        }

        // --- Estado de la OT
        var billable = ot.Detalles
            .Where(l => l.Status != EstadoDetalleOrdenTrabajoVenta.Anulado && l.ApprovedQuantity > 0m)
            .ToList();

        var anyDelivered = ot.Detalles.Any(l => l.DeliveredQuantity > 0m);
        var complete = billable.Count > 0 && billable.All(l => l.ApprovedQuantity - l.DeliveredQuantity <= 0m);

        if (complete && ot.Status == EstadoOrdenTrabajoVenta.Activo)
        {
            ot.Status = EstadoOrdenTrabajoVenta.Cerrado;
        }
        else if (!complete && ot.Status == EstadoOrdenTrabajoVenta.Cerrado)
        {
            ot.Status = EstadoOrdenTrabajoVenta.Activo;
        }

        if (anyDelivered && ot.Stage is EtapaOrdenTrabajoVenta.AprobadaCredito or EtapaOrdenTrabajoVenta.GeneraProgramacion)
        {
            ot.Stage = EtapaOrdenTrabajoVenta.GeneraProgramacion;
        }
        else
        {
            ProgramacionOrdenTrabajo.RefreshStage(ot);
        }
    }
}
