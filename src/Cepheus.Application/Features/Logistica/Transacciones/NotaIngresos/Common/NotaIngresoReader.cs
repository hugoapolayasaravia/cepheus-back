using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;

/// <summary>Lectura de una Nota de Ingreso con todo lo necesario para armar la respuesta.</summary>
public static class NotaIngresoReader
{
    public static IQueryable<NotaIngreso> WithIncludes(IQueryable<NotaIngreso> query)
        => query
            .AsNoTracking()
            .Include(x => x.ComprobantePago)
            .Include(x => x.MotivoDevolucion)
            .Include(x => x.Proveedor)
            .Include(x => x.FormaPago)
            .Include(x => x.Detalles)
                .ThenInclude(d => d.Articulo);

    public static async Task<NotaIngresoResponse> GetAsync(
        IUnitOfWork uow, string plantaCode, string code, CancellationToken ct)
    {
        var note = await WithIncludes(uow.Logistica.Transacciones.NotaIngresos.Query())
            .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Code == code, ct);

        if (note is null)
            throw new KeyNotFoundException($"La Nota de Ingreso {plantaCode}/{code} no existe.");

        return NotaIngresoMapper.Map(note);
    }
}
