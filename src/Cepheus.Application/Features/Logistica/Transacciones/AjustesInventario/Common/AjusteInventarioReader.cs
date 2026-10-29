using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;

/// <summary>Lectura de un Ajuste de Inventario con todo lo necesario para armar la respuesta.</summary>
public static class AjusteInventarioReader
{
    public static IQueryable<AjusteInventario> WithIncludes(IQueryable<AjusteInventario> query)
        => query
            .AsNoTracking()
            .Include(x => x.Planta)
            .Include(x => x.Detalles).ThenInclude(d => d.Articulo);

    public static async Task<AjusteInventarioResponse> GetAsync(
        IUnitOfWork uow,
        string plantaCode,
        string code,
        CancellationToken ct)
    {
        var ajuste = await WithIncludes(uow.Logistica.Transacciones.AjustesInventario.Query())
            .FirstOrDefaultAsync(
                x => x.PlantaCode == plantaCode && x.Code == code,
                ct);

        if (ajuste is null)
        {
            throw new KeyNotFoundException(
                $"El Ajuste de Inventario {plantaCode}/{code} no existe.");
        }

        return AjusteInventarioMapper.Map(ajuste);
    }

    /// <summary>Ajuste con líneas, tracked, para modificarlo.</summary>
    public static async Task<AjusteInventario> LoadForUpdateAsync(
        IUnitOfWork uow,
        string plantaCode,
        string code,
        CancellationToken ct)
    {
        var ajuste = await uow.Logistica.Transacciones.AjustesInventario
            .Query()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(
                x => x.PlantaCode == plantaCode && x.Code == code,
                ct);

        if (ajuste is null)
        {
            throw new KeyNotFoundException(
                $"El Ajuste de Inventario {plantaCode}/{code} no existe.");
        }

        return ajuste;
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
                $"Error al guardar el Ajuste de Inventario: {detalleError}",
                ex);
        }
    }
}
