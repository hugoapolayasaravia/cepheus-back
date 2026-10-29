using System.Globalization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;

/// <summary>
/// Reglas compartidas de la Hoja de Corte, traducidas del PowerBuilder (w_ges_reg_hoja_corte) y de
/// fn_NuevaHojaCorte / ges_sp_bus_hoja_corte. Se centralizan acá para que Create / Update / líneas / anular / copiar
/// no las dupliquen.
/// </summary>
public static class HojaCorteRules
{
    public const int YearLength = 4;
    public const int CodeLength = 8;

    public static string Normalize(string value) => value.Trim().ToUpperInvariant();

    public static string? NormalizeOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    /// <summary>Código de Hoja de Corte: si son solo dígitos y tiene menos de 8, se completa con ceros a la izquierda.</summary>
    public static string NormalizeCode(string value)
    {
        var v = value.Trim();

        return v.Length > 0 && v.Length < CodeLength && v.All(char.IsDigit)
            ? v.PadLeft(CodeLength, '0')
            : v.ToUpperInvariant();
    }

    public static string? NormalizeCodeOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : NormalizeCode(value);

    /// <summary>Código de longitud fija (char(n)): si son solo dígitos y es más corto, se completa con ceros a la izquierda.</summary>
    public static string NormalizeFixed(string value, int length)
    {
        var v = value.Trim();

        return v.Length > 0 && v.Length < length && v.All(char.IsDigit)
            ? v.PadLeft(length, '0')
            : v.ToUpperInvariant();
    }

    public static string? NormalizeFixedOrNull(string? value, int length)
        => string.IsNullOrWhiteSpace(value) ? null : NormalizeFixed(value, length);

    /// <summary>Fecha de proceso (PB: gd_fprcs). Solo fecha.</summary>
    public static DateTime GetProcessDate() => DateTime.Now.Date;

    public static string YearOf(DateTime date) => date.Year.ToString(CultureInfo.InvariantCulture);

    // ---------------------------------------------------------------- correlativo

    /// <summary>
    /// Correlativo por año de 8 dígitos (fn_NuevaHojaCorte: máximo del año + 1; 00000001 si no hay).
    /// </summary>
    public static async Task<string> NextCodeAsync(
        IUnitOfWork uow,
        string year,
        CancellationToken ct)
    {
        var lastCode = await uow.Facturacion.Transacciones.HojasCorte
            .Query()
            .AsNoTracking()
            .Where(x => x.Year == year)
            .OrderByDescending(x => x.Code)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(ct);

        var next = 1;

        if (lastCode is not null && int.TryParse(lastCode, out var last))
        {
            next = last + 1;
        }

        if (next > 99999999)
        {
            throw new InvalidOperationException(
                $"Se agotó el correlativo de Hojas de Corte para el año {year}.");
        }

        return next.ToString(CultureInfo.InvariantCulture).PadLeft(CodeLength, '0');
    }

    // ---------------------------------------------------------------- estado

    /// <summary>
    /// PB (ue_set_btn_mod_dw / grabar): una Hoja de Corte Anulada o Procesada no se modifica, y una Hoja de Corte
    /// bloqueada (Ind_Modificable = 'N') tampoco. Aplica a la cabecera y a sus líneas.
    /// </summary>
    public static void EnsureEditable(HojaCorte hojaCorte)
    {
        switch (hojaCorte.Status)
        {
            case EstadoHojaCorte.Anulado:
                throw new InvalidOperationException(
                    "La Hoja de Corte se encuentra Anulada, imposible su modificación.");

            case EstadoHojaCorte.Procesado:
                throw new InvalidOperationException(
                    "La Hoja de Corte se encuentra Procesada, imposible su modificación.");
        }

        if (!hojaCorte.IsModifiable)
        {
            throw new InvalidOperationException(
                "La Hoja de Corte se encuentra bloqueada (no modificable), imposible su modificación.");
        }
    }

    // ---------------------------------------------------------------- parseo de filtros

    /// <summary>Acepta el código legacy (01, 04, 13) o el nombre del enum.</summary>
    public static bool TryParseEstado(string? value, out EstadoHojaCorte estado)
    {
        switch (value?.Trim())
        {
            case "01": estado = EstadoHojaCorte.Pendiente; return true;
            case "04": estado = EstadoHojaCorte.Anulado; return true;
            case "13": estado = EstadoHojaCorte.Procesado; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out estado) &&
                       System.Enum.IsDefined(estado);
        }
    }

    // ---------------------------------------------------------------- advertencias de línea

    /// <summary>
    /// PB (dw_1 itemchanged de longitud_hco): la longitud es irregular si no es múltiplo de 0.05.
    /// </summary>
    public static bool IsIrregularLength(decimal length)
        => length > 0m && (length * 20m) % 1m != 0m;

    /// <summary>
    /// Advertencias (no bloquean el grabado, igual que en el PB): longitud mayor al máximo del producto
    /// y longitud irregular.
    /// </summary>
    public static IEnumerable<string> LineWarnings(HojaCorteDetalle linea)
    {
        if (linea.ProductoTipoCode != HojaCorteProductoTipos.Vigueta)
        {
            yield break;
        }

        var length = linea.Length ?? 0m;
        var limit = linea.Producto?.LengthLimit ?? 0m;

        if (limit > 0m && length > limit)
        {
            yield return FormattableString.Invariant(
                $"Paño {linea.Panel}: la longitud {length:0.00} supera el máximo de {limit:0.00} ml del producto {linea.ProductoCode}.");
        }

        if (IsIrregularLength(length))
        {
            yield return FormattableString.Invariant(
                $"Paño {linea.Panel}: la longitud {length:0.00} es irregular (no es múltiplo de 0.05).");
        }
    }
}
