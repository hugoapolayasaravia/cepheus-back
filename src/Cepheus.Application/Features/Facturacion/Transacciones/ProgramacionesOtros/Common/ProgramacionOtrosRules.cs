using System.Globalization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;

public static class ProgramacionOtrosRules
{
    public const int CodeLength = 8;

    public static string NormalizeCode(string value) => ProgramacionOrdenTrabajo.NormalizeCode(value, CodeLength);

    /// <summary>Líneas de "Otros": todas las que no son del negocio Concreto.</summary>
    public static bool IsOtros(OrdenTrabajoVentaDetalle line)
        => line.NegocioCode != OrdenTrabajoVentaRules.NegocioConcreto;

    /// <summary>Correlativo GLOBAL de 8 dígitos (fn_NuevaProgramacion_OT_Otros).</summary>
    public static async Task<string> NextCodeAsync(IUnitOfWork uow, CancellationToken ct)
    {
        var last = await uow.Facturacion.Transacciones.ProgramacionesOtros
            .Query().AsNoTracking()
            .OrderByDescending(x => x.Code)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(ct);

        var next = 1;

        if (last is not null && int.TryParse(last, NumberStyles.None, CultureInfo.InvariantCulture, out var n))
        {
            next = n + 1;
        }

        if (next > 99999999)
        {
            throw new InvalidOperationException("Se agotó el correlativo de Programaciones de Otros.");
        }

        return next.ToString(CultureInfo.InvariantCulture).PadLeft(CodeLength, '0');
    }

    /// <summary>true si la programación tiene guías no anuladas (el PB no deja modificar ni anular con guías).</summary>
    public static Task<bool> HasGuiasAsync(IUnitOfWork uow, string programacionCode, CancellationToken ct)
        => uow.Facturacion.Transacciones.GuiasRemisionVenta
            .Query().AsNoTracking()
            .AnyAsync(g => g.ProgramacionType == TipoProgramacionGuia.Otros
                           && g.ProgramacionCode == programacionCode
                           && g.Status != EstadoGuiaRemisionVenta.Anulado, ct);

    public static async Task<bool> HasDeliveriesAsync(IUnitOfWork uow, ProgramacionOtros p, CancellationToken ct)
        => p.Detalles.Any(d => d.DeliveredQuantity > 0m) || await HasGuiasAsync(uow, p.Code, ct);

    public static async Task EnsureModifiableAsync(IUnitOfWork uow, ProgramacionOtros p, string operation, CancellationToken ct)
    {
        if (p.Status != EstadoProgramacion.Pendiente)
        {
            throw new InvalidOperationException($"La programación {p.Code} está {p.Status}; no se puede {operation}.");
        }

        if (await HasDeliveriesAsync(uow, p, ct))
        {
            throw new InvalidOperationException($"La programación {p.Code} tiene guías: no se puede {operation}.");
        }
    }

    /// <summary>
    /// Cantidad programada por línea en OTRAS programaciones de Otros no anuladas de la OT (el PB sumaba sin filtrar por
    /// programación ni estado; aquí se corrige).
    /// </summary>
    public static async Task<Dictionary<int, decimal>> OtherProgrammedAsync(
        IUnitOfWork uow,
        string otCode,
        string excludeProgramacionCode,
        CancellationToken ct)
    {
        var rows = await uow.Facturacion.Transacciones.ProgramacionesOtrosDetalle
            .Query().AsNoTracking()
            .Where(d => d.OrdenTrabajoCode == otCode
                        && d.ProgramacionCode != excludeProgramacionCode
                        && d.Programacion.Status != EstadoProgramacion.Anulada)
            .GroupBy(d => d.Item)
            .Select(g => new { Item = g.Key, Qty = g.Sum(x => x.Quantity) })
            .ToListAsync(ct);

        return rows.ToDictionary(r => r.Item, r => r.Qty);
    }

    public static async Task ApplyHeaderAsync(
        IUnitOfWork uow,
        ProgramacionOtros p,
        ProgramacionOtrosFields f,
        CancellationToken ct)
    {
        ProgramacionOrdenTrabajo.EnsureDateTime(f.ProgramDate, f.ProgramTime);

        var plantCode = OrdenTrabajoVentaRules.NormalizeFixed(f.PlantCode, 2);

        var planta = await uow.Comunes.Plantas.Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == plantCode, ct)
            ?? throw new KeyNotFoundException($"La planta {plantCode} no existe.");

        if (!planta.IsActive)
        {
            throw new InvalidOperationException($"La planta {plantCode} está inactiva.");
        }

        p.ProgramDate = f.ProgramDate.Date;
        p.ProgramTime = f.ProgramTime.Trim();
        p.PlantCode = plantCode;
        p.TransportObservations = OrdenTrabajoVentaRules.NormalizeText(f.TransportObservations);
    }

    public static ProgramacionOtrosDetalle NewLine(ProgramacionOtros p, ProgramacionOtrosDetalleRequest r)
        => new()
        {
            ProgramacionCode = p.Code,
            OrdenTrabajoCode = p.OrdenTrabajoCode,
            Item = r.Item,
            Quantity = r.Quantity,
            DeliveredQuantity = 0m
        };

    /// <summary>
    /// Valida el conjunto final de líneas (PB: ue_set_btn_grb_data): líneas de la OT que no son de Concreto, no anuladas,
    /// cantidad &gt; 0 y no mayor a lo aprobado menos lo programado en otras programaciones.
    /// </summary>
    public static void Validate(OrdenTrabajoVenta ot, ProgramacionOtros p, IReadOnlyDictionary<int, decimal> others)
    {
        if (p.Detalles.Count == 0)
        {
            throw new InvalidOperationException("La programación debe tener al menos una línea.");
        }

        foreach (var l in p.Detalles)
        {
            var otLine = ot.Detalles.FirstOrDefault(d => d.Item == l.Item)
                         ?? throw new InvalidOperationException($"La línea {l.Item} no pertenece a la Orden de Trabajo {ot.Code}.");

            if (otLine.Status == EstadoDetalleOrdenTrabajoVenta.Anulado)
            {
                throw new InvalidOperationException($"La línea {l.Item} de la OT está anulada.");
            }

            if (!IsOtros(otLine))
            {
                throw new InvalidOperationException(
                    $"La línea {l.Item} es del negocio Concreto: use la programación de Concreto.");
            }

            if (l.Quantity <= 0m)
            {
                throw new InvalidOperationException($"{otLine.Producto.Name}: la cantidad debe ser mayor a cero.");
            }

            var available = otLine.ApprovedQuantity - others.GetValueOrDefault(l.Item);

            if (l.Quantity > available)
            {
                throw new InvalidOperationException(FormattableString.Invariant(
                    $"{otLine.Producto.Name}: cantidad mayor al saldo actual ({available:0.##})."));
            }
        }
    }

    /// <summary>
    /// Actualiza la cantidad programada de las líneas de Otros de la OT (= otras programaciones + esta) y la etapa de
    /// la OT (PB: pedidodetalle.cantidad_pro; estado G/C).
    /// </summary>
    public static void Recalc(OrdenTrabajoVenta ot, ProgramacionOtros p, IReadOnlyDictionary<int, decimal> others)
    {
        var mine = p.Status == EstadoProgramacion.Anulada
            ? new Dictionary<int, decimal>()
            : p.Detalles.GroupBy(d => d.Item).ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        foreach (var l in ot.Detalles.Where(IsOtros))
        {
            l.ProgrammedQuantity = others.GetValueOrDefault(l.Item) + mine.GetValueOrDefault(l.Item);
        }

        ProgramacionOrdenTrabajo.RefreshStage(ot);
    }
}
