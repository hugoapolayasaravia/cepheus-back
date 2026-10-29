using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;

/// <summary>
/// Reglas compartidas del Ajuste de Inventario, traducidas del PowerBuilder (w_log_ajustes) y de los SP Logi_sp_*.
/// Se centralizan acá para que Create / Update / líneas / procesar / devolver / anular no las dupliquen.
/// </summary>
public static class AjusteInventarioRules
{
    public const int CodeLength = 6;

    public static string Normalize(string value) => value.Trim().ToUpperInvariant();

    public static string? NormalizeOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    public static DateTime GetProcessDate() => DateTime.Now.Date;

    // ---------------------------------------------------------------- períodos y fechas

    /// <summary>
    /// Cierre parcial (CIERRE_P del PB): no se puede registrar / modificar / procesar / devolver un ajuste
    /// cuya fecha de entrega cae en un período ya cerrado para la planta (ControlCierre).
    /// Es la misma regla que usan la Nota de Ingreso y el Vale de Salida.
    /// </summary>
    public static async Task EnsurePeriodOpenAsync(
        IUnitOfWork uow,
        string plantaCode,
        DateTime fechaEntrega,
        string mensaje,
        CancellationToken ct)
    {
        // Regla única de período cerrado (la misma que aplica el servicio central de stock).
        await Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common.ControlCierreRules
            .EnsurePeriodOpenAsync(uow, plantaCode, fechaEntrega, mensaje, ct);
    }

    /// <summary>La fecha de entrega no puede ser mayor a la del sistema ni caer en un período cerrado.</summary>
    public static async Task<DateTime> ValidateFechaEntregaAsync(
        IUnitOfWork uow,
        string plantaCode,
        DateTime fechaEntrega,
        CancellationToken ct)
    {
        var entrega = fechaEntrega.Date;

        if (entrega > GetProcessDate())
        {
            throw new InvalidOperationException(
                "La Fecha de Entrega no puede ser mayor a la fecha del sistema.");
        }

        await EnsurePeriodOpenAsync(
            uow,
            plantaCode,
            entrega,
            "La Fecha de Entrega no puede ser menor o igual al cierre del período.",
            ct);

        return entrega;
    }

    // ---------------------------------------------------------------- correlativo

    /// <summary>Correlativo por planta de 6 dígitos (fg_buscarllave("majustes", planta)).</summary>
    public static async Task<string> NextCodeAsync(
        IUnitOfWork uow,
        string plantaCode,
        CancellationToken ct)
    {
        var lastCode = await uow.Logistica.Transacciones.AjustesInventario
            .Query()
            .AsNoTracking()
            .Where(x => x.PlantaCode == plantaCode)
            .OrderByDescending(x => x.Code)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(ct);

        var next = 1;

        if (lastCode is not null && int.TryParse(lastCode, out var last))
        {
            next = last + 1;
        }

        if (next > 999999)
        {
            throw new InvalidOperationException(
                $"Se agotó el correlativo de Ajuste de Inventario para la planta {plantaCode}.");
        }

        return next.ToString().PadLeft(CodeLength, '0');
    }

    // ---------------------------------------------------------------- importes

    public static decimal LineTotal(decimal cantidad, decimal precio)
        => Math.Round(cantidad * precio, 2, MidpointRounding.AwayFromZero);

    /// <summary>
    /// ue_actualiza_vales (compartido por el PB con el vale): Neto = suma de las líneas pendientes o
    /// procesadas (legacy 01 y 13; sobrantes y faltantes se suman sin signo); Igv = Neto × IGV vigente;
    /// Total = Neto + Igv. No ejecuta SaveChanges.
    /// </summary>
    public static async Task RecalculateTotalsAsync(
        IUnitOfWork uow,
        AjusteInventario ajuste,
        CancellationToken ct)
    {
        var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(uow, ajuste.FechaEntrega, ct);

        var suma = ajuste.Detalles
            .Where(d => d.Estado == EstadoAjusteInventarioDetalle.Pendiente ||
                        d.Estado == EstadoAjusteInventarioDetalle.Procesado)
            .Sum(d => d.Total);

        suma = Math.Round(suma, 2, MidpointRounding.AwayFromZero);

        var igv = Math.Round(
            suma * igvPercentage / 100m,
            2,
            MidpointRounding.AwayFromZero);

        ajuste.Neto = suma;
        ajuste.Igv = igv;
        ajuste.Total = suma + igv;
    }

    // ---------------------------------------------------------------- reservado

    /// <summary>
    /// Reservado de un artículo en el almacén: FALTANTES de ajustes pendientes MÁS líneas de vales pendientes
    /// (las salidas que todavía no restaron stock; Logi_sp_Consulta_Pendiente_Articulos suma ambos). Los sobrantes
    /// no reservan: son un ingreso.
    /// </summary>
    public static async Task<decimal> GetReservedAsync(
        IUnitOfWork uow,
        string plantaCode,
        string articuloCode,
        CancellationToken ct)
    {
        var enAjustes = await uow.Logistica.Transacciones.AjusteInventarioDetalles
            .Query()
            .AsNoTracking()
            .Where(d => d.PlantaCode == plantaCode &&
                        d.ArticuloCode == articuloCode &&
                        d.Tipo == TipoAjusteInventario.Faltante &&
                        d.Estado == EstadoAjusteInventarioDetalle.Pendiente)
            .SumAsync(d => (decimal?)d.Cantidad, ct) ?? 0m;

        var enVales = await uow.Logistica.Transacciones.ValeDetalles
            .Query()
            .AsNoTracking()
            .Where(d => d.PlantaCode == plantaCode &&
                        d.ArticuloCode == articuloCode &&
                        d.Estado == EstadoValeDetalle.Pendiente)
            .SumAsync(d => (decimal?)d.Cantidad, ct) ?? 0m;

        return enAjustes + enVales;
    }

    // ---------------------------------------------------------------- parseo de filtros

    /// <summary>Acepta el código legacy (01, 12, 13, 14, 04) o el nombre del enum.</summary>
    public static bool TryParseEstado(string? value, out EstadoAjusteInventario estado)
    {
        switch (value?.Trim())
        {
            case "01": estado = EstadoAjusteInventario.Pendiente; return true;
            case "12": estado = EstadoAjusteInventario.EntregaParcial; return true;
            case "13": estado = EstadoAjusteInventario.Procesado; return true;
            case "14": estado = EstadoAjusteInventario.Devuelto; return true;
            case "04": estado = EstadoAjusteInventario.Anulado; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out estado) &&
                       System.Enum.IsDefined(estado);
        }
    }

    /// <summary>Acepta el código legacy (I = sobrante, S = faltante) o el nombre del enum.</summary>
    public static bool TryParseTipo(string? value, out TipoAjusteInventario tipo)
    {
        switch (value?.Trim().ToUpperInvariant())
        {
            case "I": tipo = TipoAjusteInventario.Sobrante; return true;
            case "S": tipo = TipoAjusteInventario.Faltante; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out tipo) &&
                       System.Enum.IsDefined(tipo);
        }
    }

    /// <summary>Las líneas se agregan en Pendiente o Entrega Parcial (PB: no hay aprobación que saltarse).</summary>
    public static void EnsureLineasAgregables(AjusteInventario ajuste)
    {
        if (ajuste.Estado is not (EstadoAjusteInventario.Pendiente or EstadoAjusteInventario.EntregaParcial))
        {
            throw new InvalidOperationException(
                $"El Ajuste de Inventario se encuentra {ajuste.Estado}; solo se modifica en Pendiente o Entrega Parcial.");
        }
    }
}
