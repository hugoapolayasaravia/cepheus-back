using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.Common;

/// <summary>
/// Porcentaje de IGV por fecha (el fg_igv(fecha) del legacy). Es la única fuente del porcentaje de IGV en Logística:
/// ya no se lee ControlVentas.IgvPercentage.
/// </summary>
public static class PeriodoIgvRules
{
    /// <summary>
    /// Porcentaje vigente en la fecha indicada. Si no hay un período que la contenga, falla: aplicar un valor por
    /// defecto equivaldría a calcular con una tasa equivocada sin avisar.
    /// </summary>
    public static async Task<decimal> GetPorcentajeAsync(IUnitOfWork uow, DateTime fecha, CancellationToken ct)
    {
        var dia = DateOnly.FromDateTime(fecha);

        var porcentaje = await uow.Comunes.PeriodosIgv.Query()
            .AsNoTracking()
            .Where(p => p.FechaInicio <= dia && (p.FechaFin == null || p.FechaFin >= dia))
            .Select(p => (decimal?)p.PorcentajeIgv)
            .FirstOrDefaultAsync(ct);

        return porcentaje
            ?? throw new InvalidOperationException(
                $"No hay un período de IGV vigente para la fecha {dia:dd/MM/yyyy}. " +
                "Registre el período en Comunes > Períodos de IGV.");
    }
}
