using System.Globalization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;

/// <summary>
/// Reglas únicas sobre períodos cerrados (ControlCierre). Un período AAAAMM está cerrado para una planta
/// cuando existe un ControlCierre de ese período o de uno posterior (mismo criterio que el CIERRE_P del
/// PowerBuilder). Todas las transacciones de Logística que mueven stock (Nota de Ingreso, Importaciones,
/// Orden de Servicio, Vales de Salida, Ajustes) llegan a esta regla a través de ArticuloStockMovementService,
/// de modo que ningún documento nuevo puede esquivar el control.
/// </summary>
public static class ControlCierreRules
{
    public static string PeriodOf(DateTime date) =>
        date.ToString("yyyyMM", CultureInfo.InvariantCulture);

    /// <summary>Período siguiente a uno AAAAMM (202412 -> 202501).</summary>
    public static string NextPeriod(string period)
    {
        var (year, month) = Parse(period);
        return month == 12
            ? $"{year + 1:0000}01"
            : $"{year:0000}{month + 1:00}";
    }

    /// <summary>Período anterior a uno AAAAMM (202501 -> 202412).</summary>
    public static string PreviousPeriod(string period)
    {
        var (year, month) = Parse(period);
        return month == 1
            ? $"{year - 1:0000}12"
            : $"{year:0000}{month - 1:00}";
    }

    public static bool IsValidPeriod(string? period)
    {
        if (period is null || period.Length != 6 || !period.All(char.IsDigit))
            return false;

        var month = int.Parse(period.AsSpan(4, 2), CultureInfo.InvariantCulture);
        return month is >= 1 and <= 12;
    }

    /// <summary>Último período cerrado de la planta, o null si nunca se cerró ninguno.</summary>
    public static async Task<string?> GetLastClosedPeriodAsync(
        IUnitOfWork uow, string plantaCode, CancellationToken ct)
    {
        return await uow.Logistica.Maestros.ControlCierres.Query()
            .AsNoTracking()
            .Where(c => c.PlantaCode == plantaCode)
            .OrderByDescending(c => c.PeriodCode)
            .Select(c => c.PeriodCode)
            .FirstOrDefaultAsync(ct);
    }

    public static async Task<bool> IsPeriodClosedAsync(
        IUnitOfWork uow, string plantaCode, DateTime fecha, CancellationToken ct)
    {
        var period = PeriodOf(fecha);

        return await uow.Logistica.Maestros.ControlCierres.Query()
            .AsNoTracking()
            .AnyAsync(c => c.PlantaCode == plantaCode && string.Compare(c.PeriodCode, period) >= 0, ct);
    }

    public static async Task EnsurePeriodOpenAsync(
        IUnitOfWork uow, string plantaCode, DateTime fecha, string mensaje, CancellationToken ct)
    {
        if (await IsPeriodClosedAsync(uow, plantaCode, fecha, ct))
            throw new InvalidOperationException(mensaje);
    }

    private static (int Year, int Month) Parse(string period)
    {
        if (!IsValidPeriod(period))
            throw new ArgumentException($"El período '{period}' no tiene el formato AAAAMM.", nameof(period));

        return (
            int.Parse(period.AsSpan(0, 4), CultureInfo.InvariantCulture),
            int.Parse(period.AsSpan(4, 2), CultureInfo.InvariantCulture));
    }
}
