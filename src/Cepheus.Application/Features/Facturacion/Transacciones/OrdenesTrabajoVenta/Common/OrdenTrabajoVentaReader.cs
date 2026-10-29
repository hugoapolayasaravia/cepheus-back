using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>Lectura de una OT de Ventas con todo lo necesario para armar la respuesta.</summary>
public static class OrdenTrabajoVentaReader
{
    public static IQueryable<OrdenTrabajoVenta> WithIncludes(IQueryable<OrdenTrabajoVenta> query)
        => query
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Obra)
            .Include(x => x.Vendedor)
            .Include(x => x.Motivo)
            .Include(x => x.Detalles).ThenInclude(d => d.Producto).ThenInclude(p => p.Unit)
            .Include(x => x.Aprobacion).ThenInclude(a => a!.Plant1)
            .Include(x => x.Aprobacion).ThenInclude(a => a!.Plant2)
            .Include(x => x.Aprobacion).ThenInclude(a => a!.Bank)
            .Include(x => x.Aprobacion).ThenInclude(a => a!.FormaPagoVenta);

    public static async Task<OrdenTrabajoVentaResponse> GetAsync(
        IUnitOfWork uow,
        string code,
        CancellationToken ct)
    {
        var ot = await WithIncludes(uow.Facturacion.Transacciones.OrdenesTrabajoVenta.Query())
            .FirstOrDefaultAsync(x => x.Code == code, ct);

        if (ot is null)
        {
            throw new KeyNotFoundException($"La Orden de Trabajo {code} no existe.");
        }

        var hojas = await uow.Facturacion.Transacciones.HojasCorte
            .Query()
            .AsNoTracking()
            .Where(h => h.OrdenTrabajoCode == code)
            .OrderBy(h => h.Year).ThenBy(h => h.Code)
            .ToListAsync(ct);

        var igv = await OrdenTrabajoVentaRules.GetIgvRateAsync(uow, ct);

        return OrdenTrabajoVentaMapper.Map(ot, hojas, igv);
    }

    /// <summary>OT con líneas, tracked, para modificarla.</summary>
    public static async Task<OrdenTrabajoVenta> LoadForUpdateAsync(
        IUnitOfWork uow,
        string code,
        CancellationToken ct)
    {
        var ot = await uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(x => x.Code == code, ct);

        if (ot is null)
        {
            throw new KeyNotFoundException($"La Orden de Trabajo {code} no existe.");
        }

        return ot;
    }

    public static async Task SaveAsync(IUnitOfWork uow, CancellationToken ct)
    {
        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // El middleware lo traduce a 409 (otro usuario cambió la OT, la planta o la cotización).
            throw;
        }
        catch (DbUpdateException ex)
        {
            var detalleError = ex.InnerException?.Message ?? ex.Message;

            throw new InvalidOperationException(
                $"Error al guardar la Orden de Trabajo: {detalleError}",
                ex);
        }
    }
}
