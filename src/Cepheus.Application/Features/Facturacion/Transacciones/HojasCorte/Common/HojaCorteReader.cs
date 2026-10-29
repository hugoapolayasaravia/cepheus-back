using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;

/// <summary>Lectura de una Hoja de Corte con todo lo necesario para armar la respuesta.</summary>
public static class HojaCorteReader
{
    public static IQueryable<HojaCorte> WithIncludes(IQueryable<HojaCorte> query)
        => query
            .AsNoTracking()
            .Include(x => x.TipoHojaCorte)
            .Include(x => x.Tecnico).ThenInclude(t => t.Trabajador)
            .Include(x => x.Vendedor)
            .Include(x => x.Detalles).ThenInclude(d => d.Producto)
            .Include(x => x.Detalles).ThenInclude(d => d.Unit);

    public static async Task<HojaCorteResponse> GetAsync(
        IUnitOfWork uow,
        string year,
        string code,
        CancellationToken ct)
    {
        var hojaCorte = await WithIncludes(uow.Facturacion.Transacciones.HojasCorte.Query())
            .FirstOrDefaultAsync(
                x => x.Year == year && x.Code == code,
                ct);

        if (hojaCorte is null)
        {
            throw new KeyNotFoundException(
                $"La Hoja de Corte {year}/{code} no existe.");
        }

        return HojaCorteMapper.Map(hojaCorte);
    }

    /// <summary>Hoja de Corte con líneas, tracked, para modificarla.</summary>
    public static async Task<HojaCorte> LoadForUpdateAsync(
        IUnitOfWork uow,
        string year,
        string code,
        CancellationToken ct)
    {
        var hojaCorte = await uow.Facturacion.Transacciones.HojasCorte
            .Query()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(
                x => x.Year == year && x.Code == code,
                ct);

        if (hojaCorte is null)
        {
            throw new KeyNotFoundException(
                $"La Hoja de Corte {year}/{code} no existe.");
        }

        return hojaCorte;
    }

    public static async Task SaveAsync(IUnitOfWork uow, CancellationToken ct)
    {
        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            var detalleError = ex.InnerException?.Message ?? ex.Message;

            throw new InvalidOperationException(
                $"Error al guardar la Hoja de Corte: {detalleError}",
                ex);
        }
    }
}
