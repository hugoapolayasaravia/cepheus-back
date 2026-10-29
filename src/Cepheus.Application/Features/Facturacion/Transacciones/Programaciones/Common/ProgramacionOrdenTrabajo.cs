using System.Text.RegularExpressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;

/// <summary>Reglas de la OT comunes a la programación de Concreto y de Otros (PB: w_ges_reg_ordenes_trabajo_p / _o).</summary>
public static class ProgramacionOrdenTrabajo
{
    /// <summary>La fecha de programación puede ser hasta 7 días anterior a hoy.</summary>
    public const int MaxPastDays = 7;

    private static readonly Regex TimeRegex = new(@"^(0[1-9]|1\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

    public static string NormalizeCode(string value, int length = 8) => value.Trim().PadLeft(length, '0');

    /// <summary>Fecha &gt;= hoy - 7 días; hora 01-23 y minuto 00-59 (el PB rechaza la hora 00).</summary>
    public static void EnsureDateTime(DateTime date, string? time)
    {
        if (date.Date < DateTime.Today.AddDays(-MaxPastDays))
        {
            throw new InvalidOperationException(
                "La fecha de programación no puede ser menor a la fecha del sistema (se permiten hasta 7 días atrás).");
        }

        if (time is null || !TimeRegex.IsMatch(time.Trim()))
        {
            throw new InvalidOperationException("La hora de programación no es válida (HH:mm, hora 01-23 y minuto 00-59).");
        }
    }

    /// <summary>
    /// OT que se puede programar: Activa, aprobada (etapa C) o ya con programación (G), con pase a producción,
    /// crédito Aprobado, cliente no Inactivo/Suspendido y obra Activa; la obra debe tener un responsable registrado.
    /// </summary>
    public static async Task<OrdenTrabajoVenta> LoadProgramableAsync(IUnitOfWork uow, string otCode, CancellationToken ct)
    {
        var ot = await uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query()
            .Include(o => o.Cliente)
            .Include(o => o.Obra)
            .Include(o => o.Aprobacion)
            .Include(o => o.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(o => o.Code == otCode, ct)
            ?? throw new KeyNotFoundException($"La Orden de Trabajo {otCode} no existe.");

        EnsureProgramable(ot);

        var hasResponsible = await uow.Facturacion.Catalogos.ResponsablesObra
            .Query().AsNoTracking()
            .AnyAsync(r => r.ClienteCode == ot.ClienteCode && r.ObraCode == ot.ObraCode && r.Name != "", ct);

        if (!hasResponsible)
        {
            throw new InvalidOperationException("Responsable de obra no registrado: regístrelo en la obra antes de programar.");
        }

        return ot;
    }

    public static void EnsureProgramable(OrdenTrabajoVenta ot)
    {
        if (ot.Status != EstadoOrdenTrabajoVenta.Activo)
        {
            throw new InvalidOperationException($"La Orden de Trabajo está {ot.Status}; no se puede programar.");
        }

        if (ot.Stage is not (EtapaOrdenTrabajoVenta.AprobadaCredito or EtapaOrdenTrabajoVenta.GeneraProgramacion))
        {
            throw new InvalidOperationException($"La Orden de Trabajo está {ot.Stage}; solo se programa una OT aprobada.");
        }

        if (!ot.PassedToProduction)
        {
            throw new InvalidOperationException("La Orden de Trabajo no tiene pase a producción.");
        }

        if (ot.Aprobacion is null || ot.Aprobacion.CreditStatus != EstadoCreditoAprobacion.Aprobado)
        {
            throw new InvalidOperationException("La aprobación de Créditos de la Orden de Trabajo no está Aprobada.");
        }

        if (ot.Cliente.Estado is EstadoCliente.Inactivo or EstadoCliente.Suspendido)
        {
            throw new InvalidOperationException($"El cliente {ot.ClienteCode} está {ot.Cliente.Estado}; no se puede programar.");
        }

        if (ot.Obra.Estado != EstadoObra.Activo)
        {
            throw new InvalidOperationException($"La obra {ot.ClienteCode}/{ot.ObraCode} está {ot.Obra.Estado}; no se puede programar.");
        }
    }

    /// <summary>Cantidad aprobada por Créditos que aún no está en una programación.</summary>
    public static decimal Available(OrdenTrabajoVentaDetalle line) => line.ApprovedQuantity - line.ProgrammedQuantity;

    /// <summary>
    /// Etapa de la OT según sus programaciones: G (Genera programación) si alguna línea tiene cantidad programada,
    /// C (aprobada) si no queda ninguna (PB: al anular la última programación la OT vuelve a C).
    /// </summary>
    public static void RefreshStage(OrdenTrabajoVenta ot)
    {
        if (ot.Stage is not (EtapaOrdenTrabajoVenta.AprobadaCredito or EtapaOrdenTrabajoVenta.GeneraProgramacion))
        {
            return;
        }

        var any = ot.Detalles.Any(d => d.Status != EstadoDetalleOrdenTrabajoVenta.Anulado && d.ProgrammedQuantity > 0m);

        ot.Stage = any ? EtapaOrdenTrabajoVenta.GeneraProgramacion : EtapaOrdenTrabajoVenta.AprobadaCredito;
    }

    public static async Task SaveAsync(IUnitOfWork uow, string what, CancellationToken ct)
    {
        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(
                $"Error al guardar {what}: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }
}
