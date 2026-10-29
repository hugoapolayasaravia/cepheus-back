using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;

/// <summary>Operaciones sobre las guías de un comprobante (cargar, liberar, anular, recalcular saldos).</summary>
public static class ComprobanteVentaGuias
{
    /// <summary>Guías (tracked, con líneas) referenciadas por las líneas del comprobante.</summary>
    public static async Task<List<GuiaRemisionVenta>> LoadAsync(IUnitOfWork uow, ComprobanteVenta c, CancellationToken ct)
    {
        var numbers = c.Detalles.Where(d => d.GuiaNumber != null).Select(d => d.GuiaNumber!).Distinct().ToList();

        if (numbers.Count == 0)
        {
            return new List<GuiaRemisionVenta>();
        }

        return await uow.Facturacion.Transacciones.GuiasRemisionVenta
            .Query()
            .Include(g => g.Detalles)
            .Where(g => numbers.Contains(g.Number))
            .ToListAsync(ct);
    }

    /// <summary>
    /// Devuelve la guía a Pendiente. Conserva el tipo y número de documento solo si Créditos los aprobó para la OT
    /// (OT al contado: el documento ya estaba asignado desde la guía); si no, los limpia.
    /// </summary>
    public static void Release(GuiaRemisionVenta g, ComprobanteVenta c, OrdenTrabajoVenta? ot)
    {
        g.Status = EstadoGuiaRemisionVenta.Pendiente;

        var approved = ot?.Aprobacion is { } a
                       && (a.Document1Number == c.Number || a.Document2Number == c.Number);

        if (!approved)
        {
            g.DocumentTypeCode = null;
            g.Document1Number = null;
        }
    }

    public static void Cancel(GuiaRemisionVenta g, string? by, string reason)
    {
        g.Status = EstadoGuiaRemisionVenta.Anulado;
        g.CanceledAt = DateTime.Now;
        g.CanceledBy = by;
        g.CancelReason = reason;
    }

    /// <summary>Recalcula entregado/estado de la OT con las guías indicadas (si el comprobante es de una OT).</summary>
    public static async Task RecalcOrderAsync(
        IUnitOfWork uow,
        OrdenTrabajoVenta? ot,
        IReadOnlyCollection<GuiaRemisionVenta> guias,
        CancellationToken ct)
    {
        if (ot is not null && guias.Count > 0)
        {
            await GuiaRemisionVentaSaldos.RecalcAsync(uow, ot, guias, ct);
        }
    }

    /// <summary>Un comprobante solo se modifica Pendiente y sin asiento de provisión.</summary>
    public static void EnsureModifiable(ComprobanteVenta c, bool structural)
    {
        if (c.Status != EstadoComprobanteVenta.Pendiente)
        {
            throw new InvalidOperationException(
                $"El comprobante {ComprobanteVentaRules.TypeName(c.TypeCode)} {c.Number} está {c.Status}; solo se modifica si está Pendiente.");
        }

        if (!string.IsNullOrWhiteSpace(c.ProvisionEntry))
        {
            throw new InvalidOperationException($"El comprobante {c.Number} ya tiene asiento de provisión ({c.ProvisionEntry}); no se modifica.");
        }

        if (structural && c.IsPrinted)
        {
            throw new InvalidOperationException($"El comprobante {c.Number} ya está impreso; no se pueden cambiar sus líneas o guías.");
        }
    }
}
