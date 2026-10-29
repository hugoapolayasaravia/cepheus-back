using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;

/// <summary>Lectura de una Orden de Servicio con todo lo necesario para armar la respuesta.</summary>
public static class OrdenServicioReader
{
    public static IQueryable<OrdenServicio> WithIncludes(IQueryable<OrdenServicio> query)
        => query
            .AsNoTracking()
            .Include(x => x.Planta)
            .Include(x => x.ComprobantePago)
            .Include(x => x.Proveedor)
            .Include(x => x.FormaPago)
            .Include(x => x.Comprador)
            .Include(x => x.LugarEnvio)
            .Include(x => x.Tramite)
            .Include(x => x.NotaCompra)
            .Include(x => x.UnidadNegocio)
            .Include(x => x.ValeSalida).ThenInclude(v => v.Trabajador)
            .Include(x => x.Detalles).ThenInclude(d => d.Articulo)
            .Include(x => x.Detalles).ThenInclude(d => d.TipoVale)
            .Include(x => x.Detalles).ThenInclude(d => d.SubCentroCosto)
            .Include(x => x.Detalles).ThenInclude(d => d.SubCentroEjecutor)
            .AsSplitQuery();

    public static async Task<OrdenServicioResponse> GetAsync(
        IUnitOfWork uow, string plantaCode, string code, CancellationToken ct)
    {
        var os = await WithIncludes(uow.Logistica.Transacciones.OrdenesServicio.Query())
            .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Code == code, ct);

        if (os is null)
            throw new KeyNotFoundException($"La Orden de Servicio {plantaCode}/{code} no existe.");

        return OrdenServicioMapper.Map(os);
    }
}
