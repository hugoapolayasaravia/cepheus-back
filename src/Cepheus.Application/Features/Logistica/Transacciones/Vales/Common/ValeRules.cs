using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;

/// <summary>
/// Reglas compartidas del Vale de Salida, traducidas del PowerBuilder (w_vales) y de los SP Logi_sp_*.
/// Se centralizan acá para que Create / Update / líneas / procesar / devolver / anular no las dupliquen.
/// </summary>
public static class ValeRules
{
    // Códigos legacy de TipoVale que condicionan el flujo.
    public const string TipoValeConOt = "002"; //"COT";
    public const string TipoValeConOtPreventivo = "003"; //"CPE";
    public const string TipoValeInterno = "001"; //"CIN";
    public const string TipoValeCombustible = "006"; //"VSC";
    public const string TipoValeTransferencia = "005"; //"TRA";

    // Tipos de transacción para el esquema de aprobación (Logi_sp_Verifica_Aprobacion_Transaccion).
    public const string TipoTransaccionVale = "VS";
    public const string TipoTransaccionValeCliente = "VC";

    // El vale se valoriza siempre en soles.
    public const string MonedaSoles = "PEN";

    public const string CodePrefix = "9";
    public const int CorrelativeLength = 6;

    // Estados de OTRMaterial (TOTRMateriales.Codigo_est).
    public const string OtMaterialPendiente = "01";
    public const string OtMaterialAsignado = "15";
    public const string OtMaterialConsumido = "13";

    public static string Normalize(string value) => value.Trim().ToUpperInvariant();

    public static string? NormalizeOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    public static DateTime GetProcessDate() => DateTime.Now.Date;

    public static string TipoTransaccion(string tipoValeCode)
        => tipoValeCode == TipoValeCombustible
            ? TipoTransaccionValeCliente
            : TipoTransaccionVale;

    // ---------------------------------------------------------------- períodos

    /// <summary>
    /// Cierre parcial (CIERRE_P del PB): no se puede registrar / modificar / procesar / devolver un vale
    /// cuya fecha de entrega cae en un período ya cerrado para la planta (ControlCierre).
    /// Es la misma regla que usa la Nota de Ingreso.
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

    // ---------------------------------------------------------------- correlativo

    /// <summary>Correlativo por planta con formato 9XXXXXX (fg_buscarllave("mvales", planta)).</summary>
    public static async Task<string> NextCodeAsync(
        IUnitOfWork uow,
        string plantaCode,
        CancellationToken ct)
    {
        var lastCode = await uow.Logistica.Transacciones.Vales
            .Query()
            .AsNoTracking()
            .Where(x => x.PlantaCode == plantaCode && x.Code.StartsWith(CodePrefix))
            .OrderByDescending(x => x.Code)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(ct);

        var next = 1;

        if (lastCode is not null &&
            int.TryParse(lastCode.Substring(CodePrefix.Length), out var last))
        {
            next = last + 1;
        }

        if (next > 999999)
        {
            throw new InvalidOperationException(
                $"Se agotó el correlativo de Vale de Salida para la planta {plantaCode}.");
        }

        return CodePrefix + next.ToString().PadLeft(CorrelativeLength, '0');
    }

    // ---------------------------------------------------------------- importes

    public static decimal LineTotal(decimal cantidad, decimal precio)
        => Math.Round(cantidad * precio, 2, MidpointRounding.AwayFromZero);

    /// <summary>
    /// ue_actualiza_vales: Neto = suma de las líneas pendientes o procesadas (legacy 01, 13 y 09);
    /// Igv = Neto × IGV vigente; Total = Neto + Igv. No ejecuta SaveChanges.
    /// </summary>
    public static async Task RecalculateTotalsAsync(
        IUnitOfWork uow,
        Vale vale,
        CancellationToken ct)
    {
        var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(uow, vale.FechaEntrega, ct);

        var suma = vale.Detalles
            .Where(d => d.Estado == EstadoValeDetalle.Pendiente ||
                        d.Estado == EstadoValeDetalle.Procesado)
            .Sum(d => d.Total);

        suma = Math.Round(suma, 2, MidpointRounding.AwayFromZero);

        var igv = Math.Round(
            suma * igvPercentage / 100m,
            2,
            MidpointRounding.AwayFromZero);

        vale.Neto = suma;
        vale.Igv = igv;
        vale.Total = suma + igv;
    }

    // ---------------------------------------------------------------- stock reservado

    /// <summary>
    /// Reservado de un artículo en un almacén (Logi_sp_Consulta_Pendiente_Articulos): cantidad de las líneas
    /// de vale en estado Pendiente MÁS los faltantes de ajustes de inventario pendientes (ambas son salidas que
    /// todavía no restaron stock; el legacy suma las dos).
    /// </summary>
    public static async Task<decimal> GetReservedAsync(
        IUnitOfWork uow,
        string plantaCode,
        string articuloCode,
        CancellationToken ct)
    {
        var enVales = await uow.Logistica.Transacciones.ValeDetalles
            .Query()
            .AsNoTracking()
            .Where(d => d.PlantaCode == plantaCode &&
                        d.ArticuloCode == articuloCode &&
                        d.Estado == EstadoValeDetalle.Pendiente)
            .SumAsync(d => (decimal?)d.Cantidad, ct) ?? 0m;

        var enAjustes = await uow.Logistica.Transacciones.AjusteInventarioDetalles
            .Query()
            .AsNoTracking()
            .Where(d => d.PlantaCode == plantaCode &&
                        d.ArticuloCode == articuloCode &&
                        d.Tipo == TipoAjusteInventario.Faltante &&
                        d.Estado == EstadoAjusteInventarioDetalle.Pendiente)
            .SumAsync(d => (decimal?)d.Cantidad, ct) ?? 0m;

        return enVales + enAjustes;
    }

    // ---------------------------------------------------------------- estados

    /// <summary>Acepta el código legacy (01, 09, 12, 13, 14, 30, 04) o el nombre del enum.</summary>
    public static bool TryParseEstado(string? value, out EstadoVale estado)
    {
        switch (value?.Trim())
        {
            case "01": estado = EstadoVale.Pendiente; return true;
            case "09": estado = EstadoVale.Aprobado; return true;
            case "30": estado = EstadoVale.AprobacionProvisional; return true;
            case "12": estado = EstadoVale.EntregaParcial; return true;
            case "13": estado = EstadoVale.Procesado; return true;
            case "14": estado = EstadoVale.Devuelto; return true;
            case "04": estado = EstadoVale.Anulado; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out estado) &&
                       System.Enum.IsDefined(estado);
        }
    }

    public static void EnsureEditable(Vale vale, string accion)
    {
        if (vale.Estado != EstadoVale.Pendiente)
        {
            throw new InvalidOperationException(
                $"El Vale de Salida se encuentra {vale.Estado}; solo se puede {accion} en estado Pendiente.");
        }
    }

    public static bool EsTransferencia(Vale vale)
        => vale.TipoValeCode == TipoValeTransferencia;
}
