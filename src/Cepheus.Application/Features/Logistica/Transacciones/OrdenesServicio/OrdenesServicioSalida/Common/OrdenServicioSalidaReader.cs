using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;

public static class OrdenServicioSalidaReader
{
    public static IQueryable<OrdenServicioSalida> WithIncludes(IQueryable<OrdenServicioSalida> query)
        => query
            .AsNoTracking()
            .Include(x => x.Planta)
            .Include(x => x.Trabajador)
            .Include(x => x.Detalles).ThenInclude(d => d.Articulo)
            .Include(x => x.Detalles).ThenInclude(d => d.TipoVale)
            .Include(x => x.Detalles).ThenInclude(d => d.SubCentroCosto)
            .Include(x => x.Detalles).ThenInclude(d => d.SubCentroEjecutor)
            .AsSplitQuery();

    public static async Task<OrdenServicioSalidaResponse> GetAsync(
        IUnitOfWork uow, string plantaCode, string code, CancellationToken ct)
    {
        var vale = await WithIncludes(uow.Logistica.Transacciones.OrdenesServicioSalida.Query())
            .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Code == code, ct);

        if (vale is null)
            throw new KeyNotFoundException($"El vale de salida {plantaCode}/{code} no existe.");

        return OrdenServicioSalidaMapper.Map(vale);
    }
}
