using System.Globalization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;

public static class ProgramacionConcretoRules
{
    public const int CodeLength = 8;
    public const string ConcretoProductoTipo = "01";
    public const string BombeoProductoTipo = "04";

    /// <summary>Valor con el que el legacy marcaba "sin bomba"; se guarda como null.</summary>
    public const string NoPump = "S/BO";

    public static string NormalizeCode(string value) => ProgramacionOrdenTrabajo.NormalizeCode(value, CodeLength);

    /// <summary>Correlativo GLOBAL de 8 dígitos (fn_NuevaProgramacion_OT_Concreto).</summary>
    public static async Task<string> NextCodeAsync(IUnitOfWork uow, CancellationToken ct)
    {
        var last = await uow.Facturacion.Transacciones.ProgramacionesConcreto
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
            throw new InvalidOperationException("Se agotó el correlativo de Programaciones de Concreto.");
        }

        return next.ToString(CultureInfo.InvariantCulture).PadLeft(CodeLength, '0');
    }

    /// <summary>true si la programación tiene guías no anuladas (el PB no deja modificar ni anular con guías).</summary>
    public static Task<bool> HasGuiasAsync(IUnitOfWork uow, string programacionCode, CancellationToken ct)
        => uow.Facturacion.Transacciones.GuiasRemisionVenta
            .Query().AsNoTracking()
            .AnyAsync(g => g.ProgramacionType == TipoProgramacionGuia.Concreto
                           && g.ProgramacionCode == programacionCode
                           && g.Status != EstadoGuiaRemisionVenta.Anulado, ct);

    public static async Task<bool> HasDeliveriesAsync(IUnitOfWork uow, ProgramacionConcreto p, CancellationToken ct)
        => p.Detalles.Any(d => d.DeliveredQuantity > 0m) || await HasGuiasAsync(uow, p.Code, ct);

    /// <summary>PB: con guías la programación no se modifica ni se anula; solo una Pendiente.</summary>
    public static async Task EnsureModifiableAsync(IUnitOfWork uow, ProgramacionConcreto p, string operation, CancellationToken ct)
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

    /// <summary>Cantidad programada por línea de la OT en OTRAS programaciones de Concreto no anuladas.</summary>
    public static async Task<Dictionary<int, decimal>> OtherProgrammedAsync(
        IUnitOfWork uow,
        string otCode,
        string excludeProgramacionCode,
        CancellationToken ct)
    {
        var rows = await uow.Facturacion.Transacciones.ProgramacionesConcretoDetalle
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
        ProgramacionConcreto p,
        ProgramacionConcretoFields f,
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

        var elementoCode = OrdenTrabajoVentaRules.NormalizeFixed(f.ElementoCode, 2);

        var elemento = await uow.Facturacion.Catalogos.ElementosConcreto.Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == elementoCode, ct)
            ?? throw new KeyNotFoundException($"El elemento {elementoCode} no existe.");

        if (!elemento.IsActive)
        {
            throw new InvalidOperationException($"El elemento {elementoCode} está inactivo.");
        }

        p.ProgramDate = f.ProgramDate.Date;
        p.ProgramTime = f.ProgramTime.Trim();
        p.PlantCode = plantCode;
        p.ElementoCode = elementoCode;
        p.TransportObservations = OrdenTrabajoVentaRules.NormalizeText(f.TransportObservations);
        p.RequiresLabService = f.RequiresLabService;
    }

    public static ProgramacionConcretoDetalle NewLine(ProgramacionConcreto p, ProgramacionConcretoDetalleRequest r)
        => new()
        {
            ProgramacionCode = p.Code,
            OrdenTrabajoCode = p.OrdenTrabajoCode,
            Item = r.Item,
            Quantity = r.Quantity,
            DeliveredQuantity = 0m,
            PumpCode = NormalizePump(r.PumpCode),
            Support = r.Support
        };

    public static string? NormalizePump(string? value)
    {
        var v = OrdenTrabajoVentaRules.NormalizeText(value);

        return v is null || string.Equals(v, NoPump, StringComparison.OrdinalIgnoreCase) ? null : v.ToUpperInvariant();
    }

    /// <summary>
    /// Valida el conjunto final de líneas de la programación (PB: ue_set_btn_grb_data): líneas de la OT del negocio CO,
    /// cantidad &gt; 0 y dentro de lo aprobado y no programado en otras programaciones, un solo concreto (tipo 01), bombeo
    /// con bomba y apoyo 0-9, misma unidad de medida en todas y ninguna cantidad mayor a la del concreto.
    /// </summary>
    public static void Validate(OrdenTrabajoVenta ot, ProgramacionConcreto p, IReadOnlyDictionary<int, decimal> others)
    {
        var lines = p.Detalles.ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException("La programación debe tener al menos una línea.");
        }

        string? unit = null;
        decimal? concreteQuantity = null;

        foreach (var l in lines)
        {
            var otLine = ot.Detalles.FirstOrDefault(d => d.Item == l.Item)
                         ?? throw new InvalidOperationException($"La línea {l.Item} no pertenece a la Orden de Trabajo {ot.Code}.");

            if (otLine.Status == EstadoDetalleOrdenTrabajoVenta.Anulado)
            {
                throw new InvalidOperationException($"La línea {l.Item} de la OT está anulada.");
            }

            if (otLine.NegocioCode != OrdenTrabajoVentaRules.NegocioConcreto)
            {
                throw new InvalidOperationException(
                    $"La línea {l.Item} es del negocio {otLine.NegocioCode}: la programación de Concreto solo admite el negocio CO (use la programación de Otros).");
            }

            if (l.Quantity <= 0m)
            {
                throw new InvalidOperationException($"{otLine.Producto.Name}: la cantidad debe ser mayor a cero.");
            }

            var available = otLine.ApprovedQuantity - others.GetValueOrDefault(l.Item);

            if (l.Quantity > available)
            {
                throw new InvalidOperationException(FormattableString.Invariant(
                    $"{otLine.Producto.Name}: la cantidad {l.Quantity:0.##} supera lo disponible por programar ({available:0.##})."));
            }

            if (otLine.ProductoTipoCode == ConcretoProductoTipo)
            {
                if (concreteQuantity is not null)
                {
                    throw new InvalidOperationException("Ya existe un producto tipo concreto en la programación.");
                }

                concreteQuantity = l.Quantity;
            }

            if (otLine.ProductoTipoCode == BombeoProductoTipo)
            {
                if (string.IsNullOrWhiteSpace(l.PumpCode))
                {
                    throw new InvalidOperationException("Bomba no registrada: el bombeo exige el código de la bomba.");
                }

                if (l.Support is null or > 9)
                {
                    throw new InvalidOperationException("Apoyo incorrecto: debe ser un número de 0 a 9.");
                }
            }
            else
            {
                l.PumpCode = null;
                l.Support = null;
            }

            var lineUnit = otLine.Producto.UnitCode;

            if (unit is not null && unit != lineUnit)
            {
                throw new InvalidOperationException("Unidad de medida diferente: todas las líneas de la programación deben compartir unidad.");
            }

            unit = lineUnit;
        }

        if (concreteQuantity is > 0m)
        {
            foreach (var l in lines)
            {
                var tipo = ot.Detalles.First(d => d.Item == l.Item).ProductoTipoCode;

                if (tipo != ConcretoProductoTipo && l.Quantity > concreteQuantity)
                {
                    throw new InvalidOperationException("Cantidad mayor a Concreto: ninguna línea puede superar la cantidad de concreto programada.");
                }
            }
        }
    }

    /// <summary>
    /// Actualiza la cantidad programada de las líneas CO de la OT (= otras programaciones + esta) y la etapa de la OT
    /// (PB: pedidodetalle.cantidad_pro = suma de ConcretoAdicional; estado G/C).
    /// </summary>
    public static void Recalc(OrdenTrabajoVenta ot, ProgramacionConcreto p, IReadOnlyDictionary<int, decimal> others)
    {
        var mine = p.Status == EstadoProgramacion.Anulada
            ? new Dictionary<int, decimal>()
            : p.Detalles.GroupBy(d => d.Item).ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        foreach (var l in ot.Detalles.Where(d => d.NegocioCode == OrdenTrabajoVentaRules.NegocioConcreto))
        {
            l.ProgrammedQuantity = others.GetValueOrDefault(l.Item) + mine.GetValueOrDefault(l.Item);
        }

        ProgramacionOrdenTrabajo.RefreshStage(ot);
    }
}
