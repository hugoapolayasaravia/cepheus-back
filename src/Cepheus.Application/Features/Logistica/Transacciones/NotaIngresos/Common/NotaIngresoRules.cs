using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;

/// <summary>
/// Reglas compartidas de la Nota de Ingreso, traducidas del PowerBuilder (w_notaingresos) y de
/// los SP Logi_sp_*. Se centralizan acá para que Create / Update / Anulate no las dupliquen.
/// </summary>
public static class NotaIngresoRules
{
    // Códigos legacy (Codigo_tdo / Codigo_Mot / Codigo_tco) que condicionan el flujo.
    public const string ComprobanteNotaCredito = "07";
    public const string ComprobanteGuia = "09";
    public const string ComprobanteHonorarios = "02";

    /// <summary>Motivo 01: ajuste de precio/descuento. No mueve stock.</summary>
    public const string MotivoAjuste = "01";

    /// <summary>Motivo 02: devolución de mercadería. Descuenta stock.</summary>
    public const string MotivoDevolucion = "02";

    /// <summary>Tipo de compra "Servicio": no ingresa por Nota de Ingreso.</summary>
    public const string TipoCompraServicio = "S";

    public const string MonedaDolar = "USD";

    public const string CodePrefix = "9";
    public const int CorrelativeLength = 5;

    /// <summary>Tope (en meses) hacia atrás para la fecha de recepción.</summary>
    public const int MesesTopeRecepcion = 4;

    public static string Normalize(string value) => value.Trim().ToUpperInvariant();

    public static string? NormalizeOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    // ---------------------------------------------------------------- fechas / períodos

    /// <summary>
    /// Fecha de proceso del sistema (gd_fprcs del PB). Se toma de ControlVentas.PurchasesProcessDate;
    /// si no está configurada se usa la fecha actual.
    /// </summary>
    public static async Task<DateTime> GetProcessDateAsync(IUnitOfWork uow, CancellationToken ct)
    {
        var control = await uow.Comunes.ControlesVentas.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        var date = control?.PurchasesProcessDate;
        return date is null || date.Value == default ? DateTime.Today : date.Value.Date;
    }

    public static DateTime GetProcessDate()
    {
        return DateTime.Now.Date;
    }

    /// <summary>
    /// Cierre parcial (CIERRE_P del PB): no se puede registrar / modificar / anular una nota cuya
    /// fecha de recepción cae en un período ya cerrado para la planta (ControlCierre).
    /// </summary>
    public static async Task EnsurePeriodOpenAsync(
        IUnitOfWork uow, string plantaCode, DateTime fechaRecepcion, string mensaje, CancellationToken ct)
    {
        // Regla única de período cerrado (la misma que aplica el servicio central de stock).
        await Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common.ControlCierreRules
            .EnsurePeriodOpenAsync(uow, plantaCode, fechaRecepcion, mensaje, ct);
    }

    /// <summary>
    /// Validaciones de fechas del evento de grabado del PB, en el mismo orden:
    /// período cerrado, tope de 4 meses de la recepción, emisión y recepción no mayores a la fecha de proceso.
    /// </summary>
    public static async Task ValidateDatesAsync(
        IUnitOfWork uow, string plantaCode, DateTime fechaEmision, DateTime fechaRecepcion, CancellationToken ct)
    {
        await EnsurePeriodOpenAsync(
            uow, plantaCode, fechaRecepcion,
            "La Fecha de Recepción no puede ser menor o igual al cierre del período.", ct);

        //var proceso = await GetProcessDateAsync(uow, ct);
        var proceso = GetProcessDate();

        // PB: mes = mes(proceso) - 4; si es negativo => 12 - |mes| del año anterior.
        var month = proceso.Month - MesesTopeRecepcion;
        var year = proceso.Year;
        if (month < 0)
        {
            month = 12 + month;
            year -= 1;
        }

        var limit = year * 100 + month;
        if (fechaRecepcion.Year * 100 + fechaRecepcion.Month <= limit)
            throw new InvalidOperationException(
                $"La Fecha de Recepción no es la correcta (máximo {MesesTopeRecepcion} meses hacia atrás).");

        if (fechaEmision.Date > proceso)
            throw new InvalidOperationException("La Fecha de Emisión no puede ser mayor a la fecha de proceso.");

        if (fechaRecepcion.Date > proceso)
            throw new InvalidOperationException("La Fecha de Recepción no puede ser mayor a la fecha de proceso.");
    }

    // ---------------------------------------------------------------- tipo de cambio

    public static bool EsDolar(string monedaCode) => monedaCode == MonedaDolar;

    /// <summary>
    /// Tipo de cambio venta del día de la fecha de emisión (solo comprobantes en dólares).
    /// En soles devuelve 0, como el legacy.
    /// </summary>
    public static async Task<decimal> ResolveTipoCambioAsync(
        IUnitOfWork uow, string monedaCode, DateTime fechaEmision, CancellationToken ct)
    {
        if (!EsDolar(monedaCode))
            return 0m;

        var date = DateOnly.FromDateTime(fechaEmision);
        var tc = await uow.Comunes.TiposCambio.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Date == date, ct);

        if (tc is null || tc.SellRate <= 0)
            throw new InvalidOperationException(
                "No se registra el tipo de cambio del día de la fecha de emisión.");

        return tc.SellRate;
    }

    // ---------------------------------------------------------------- importes por línea

    /// <summary>Total de línea del PB: Cantidad × Precio × (1 − Descuento%/100), redondeado a 2 decimales.</summary>
    public static decimal LineTotal(decimal cantidad, decimal precio, decimal descuentoPorcentaje)
    {
        var bruto = cantidad * precio;
        return Math.Round(bruto - bruto * (descuentoPorcentaje / 100m), 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Precios en soles / dólares que recibe el stock (PrecioS / PrecioD del PB).
    /// Dólares: S = round(precio × TC, 2), D = precio. Soles: S = precio, D = round(precio / TC, 2) si hay TC.
    /// </summary>
    public static (decimal Soles, decimal Dolares) ConvertPrices(string monedaCode, decimal precio, decimal tipoCambio)
    {
        if (EsDolar(monedaCode))
            return (Math.Round(precio * tipoCambio, 2, MidpointRounding.AwayFromZero), precio);

        return (precio, tipoCambio > 0
            ? Math.Round(precio / tipoCambio, 2, MidpointRounding.AwayFromZero)
            : 0m);
    }

    // ---------------------------------------------------------------- correlativo

    /// <summary>Correlativo por planta con formato 9XXXXX.</summary>
    public static async Task<string> NextCodeAsync(IUnitOfWork uow, string plantaCode, CancellationToken ct)
    {
        var lastCode = await uow.Logistica.Transacciones.NotaIngresos.Query()
            .AsNoTracking()
            .Where(x => x.PlantaCode == plantaCode && x.Code.StartsWith(CodePrefix))
            .OrderByDescending(x => x.Code)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(ct);

        var next = 1;
        if (lastCode is not null && int.TryParse(lastCode.Substring(CodePrefix.Length), out var last))
            next = last + 1;

        if (next > 99999)
            throw new InvalidOperationException(
                $"Se agotó el correlativo de Nota de Ingreso para la planta {plantaCode}.");

        return CodePrefix + next.ToString().PadLeft(CorrelativeLength, '0');
    }

    // ---------------------------------------------------------------- estado legacy

    /// <summary>
    /// Acepta el código legacy del listado (01, 13, 11, 04) o el nombre del enum.
    /// </summary>
    public static bool TryParseEstado(string? value, out EstadoNotaIngreso estado)
    {
        switch (value?.Trim())
        {
            case "01": estado = EstadoNotaIngreso.Pendiente; return true;
            case "13": estado = EstadoNotaIngreso.Procesado; return true;
            case "11": estado = EstadoNotaIngreso.Cerrado; return true;
            case "04": estado = EstadoNotaIngreso.Anulado; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out estado)
                    && System.Enum.IsDefined(estado);
        }
    }
}
