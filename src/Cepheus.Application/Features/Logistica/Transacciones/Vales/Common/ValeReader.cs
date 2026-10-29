using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;

/// <summary>Lectura de un Vale de Salida con todo lo necesario para armar la respuesta.</summary>
public static class ValeReader
{
    public static IQueryable<Vale> WithListadoIncludes(IQueryable<Vale> query)
        => query
            .AsNoTracking()
            .Include(x => x.TipoVale)
            .Include(x => x.SubCentroCosto).ThenInclude(s => s.CentroCosto)
            .Include(x => x.Trabajador)
            .Include(x => x.OrdenTrabajo)
            .Include(x => x.PlantaAfectada);

    public static IQueryable<Vale> WithIncludes(IQueryable<Vale> query)
        => WithListadoIncludes(query)
            .Include(x => x.SubCentroEjecutor)
            .Include(x => x.UnidadNegocio)
            .Include(x => x.Detalles).ThenInclude(d => d.Articulo);

    public static async Task<ValeResponse> GetAsync(
        IUnitOfWork uow,
        string plantaCode,
        string code,
        CancellationToken ct)
    {
        var vale = await WithIncludes(uow.Logistica.Transacciones.Vales.Query())
            .FirstOrDefaultAsync(
                x => x.PlantaCode == plantaCode && x.Code == code,
                ct);

        if (vale is null)
        {
            throw new KeyNotFoundException(
                $"El Vale de Salida {plantaCode}/{code} no existe.");
        }

        return ValeMapper.Map(vale);
    }

    /// <summary>Vale con líneas, tracked, para modificarlo.</summary>
    public static async Task<Vale> LoadForUpdateAsync(
        IUnitOfWork uow,
        string plantaCode,
        string code,
        CancellationToken ct)
    {
        var vale = await uow.Logistica.Transacciones.Vales
            .Query()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(
                x => x.PlantaCode == plantaCode && x.Code == code,
                ct);

        if (vale is null)
        {
            throw new KeyNotFoundException(
                $"El Vale de Salida {plantaCode}/{code} no existe.");
        }

        return vale;
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
                $"Error al guardar el Vale de Salida: {detalleError}",
                ex);
        }
    }
}
