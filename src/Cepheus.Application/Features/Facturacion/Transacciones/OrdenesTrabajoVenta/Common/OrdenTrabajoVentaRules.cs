using System.Globalization;
using System.Text.RegularExpressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>
/// Reglas compartidas de la OT de Ventas, traducidas del PowerBuilder (w_ges_reg_ordenes_trabajo,
/// w_ges_reg_det_otrabajo, w_ges_bus_orden_trabajo) y de fn_NuevaOrdenTrabajo / ges_ordenTrabajo.
/// Se centralizan acá para que Create / Update / líneas / anular / copiar / aprobar no las dupliquen.
/// </summary>
public static class OrdenTrabajoVentaRules
{
    public const int CodeLength = 8;

    /// <summary>
    /// Qué pasa con las Hojas de Corte de una OT que se anula. false (criterio del PB al anular): la HC pasa a
    /// Anulado (04) y se le quita la OT. true (criterio del reexport del PB): la HC vuelve a Pendiente (01) y queda
    /// libre para otra OT. El usuario pidió aplicar ambos criterios y revisarlos luego: cambiar acá.
    /// </summary>
    public const bool ReleaseHojasCorteAsPending = false;

    /// <summary>Largo máximo de los campos de usuario de la OT (varchar(20) como el legacy).</summary>
    public const int UserFieldLength = 20;

    public static string? UserName(Cepheus.Application.Comun.Interfaces.ICurrentUserService user)
    {
        var name = user.FullName ?? user.Email;
        if (string.IsNullOrWhiteSpace(name)) return null;
        name = name.Trim();
        return name.Length <= UserFieldLength ? name : name[..UserFieldLength];
    }

    public static DateTime Now() => DateTime.Now;

    /// <summary>Código de producto char(4).</summary>
    public static string NormalizeProductoCode(string value) => NormalizeFixed(value, 4);

    /// <summary>Código de línea de producto char(2).</summary>
    public static string NormalizeTwo(string value) => NormalizeFixed(value, 2);


    /// <summary>Tipo de producto de descuento: precio y total se guardan negativos.</summary>
    public const string DescuentoProductoTipo = "23";

    /// <summary>Negocio de Pretensados: sus OT/líneas no se bloquean por provenir de cotización ni de HC.</summary>
    public const string NegocioPretensados = "PT";

    /// <summary>Negocio de Concreto: exige cotización.</summary>
    public const string NegocioConcreto = "CO";

    /// <summary>Productos de Hoja de Corte (vigueta 16 / bovedilla 18).</summary>
    public static bool IsHojaCorteProducto(string productoTipoCode)
        => productoTipoCode == HojaCorteProductoTipos.Vigueta || productoTipoCode == HojaCorteProductoTipos.Bovedilla;

    private static readonly Regex TimeRegex = new(@"^(0[1-9]|1\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

    public static bool IsValidTime(string? value) => value is not null && TimeRegex.IsMatch(value.Trim());

    // ---------------------------------------------------------------- normalización

    public static string Normalize(string value) => value.Trim().ToUpperInvariant();

    public static string? NormalizeOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

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

    /// <summary>Código de OT (char(8)).</summary>
    public static string NormalizeCode(string value) => NormalizeFixed(value, CodeLength);

    public static string? NormalizeText(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    // ---------------------------------------------------------------- correlativo

    /// <summary>
    /// Correlativo GLOBAL de 8 dígitos (fn_NuevaOrdenTrabajo: máximo + 1; 00000001 si no hay).
    /// </summary>
    public static async Task<string> NextCodeAsync(IUnitOfWork uow, CancellationToken ct)
    {
        var lastCode = await uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query()
            .AsNoTracking()
            .OrderByDescending(x => x.Code)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(ct);

        var next = 1;

        if (lastCode is not null && int.TryParse(lastCode, NumberStyles.None, CultureInfo.InvariantCulture, out var last))
        {
            next = last + 1;
        }

        if (next > 99999999)
        {
            throw new InvalidOperationException("Se agotó el correlativo de Órdenes de Trabajo.");
        }

        return next.ToString(CultureInfo.InvariantCulture).PadLeft(CodeLength, '0');
    }

    // ---------------------------------------------------------------- estado

    /// <summary>
    /// PB (ue_validar / dw_2 doubleclicked): una OT no se modifica si está Cerrada, Inactiva, Anulada,
    /// Aprobada o con programación, ni si está bloqueada (Ind_Modificable = 'N').
    /// </summary>
    public static void EnsureEditable(OrdenTrabajoVenta ot)
    {
        EnsureActive(ot, "modificación");

        switch (ot.Stage)
        {
            case EtapaOrdenTrabajoVenta.Anulada:
                throw new InvalidOperationException("La Orden de Trabajo se encuentra Anulada, imposible su modificación.");

            case EtapaOrdenTrabajoVenta.AprobadaCredito:
                throw new InvalidOperationException(
                    "La Orden de Trabajo se encuentra aprobada, imposible su modificación. Coordinar con Créditos.");

            case EtapaOrdenTrabajoVenta.GeneraProgramacion:
                throw new InvalidOperationException(
                    "La Orden de Trabajo tiene programación(es) existente(s), imposible su modificación.");
        }

        if (!ot.IsModifiable)
        {
            throw new InvalidOperationException(
                "La Orden de Trabajo se encuentra bloqueada (no modificable), imposible su modificación.");
        }
    }

    /// <summary>La OT debe estar Activa (no Inactiva, Cerrada ni Cerrada definitivamente).</summary>
    public static void EnsureActive(OrdenTrabajoVenta ot, string operation)
    {
        switch (ot.Status)
        {
            case EstadoOrdenTrabajoVenta.Inactivo:
                throw new InvalidOperationException(
                    $"La Orden de Trabajo está inactiva, imposible su {operation}. Coordinar con Créditos.");

            case EstadoOrdenTrabajoVenta.Cerrado:
                throw new InvalidOperationException(
                    $"La Orden de Trabajo está cerrada, imposible su {operation}.");

            case EstadoOrdenTrabajoVenta.CerradoDefinitivo:
                throw new InvalidOperationException(
                    $"La Orden de Trabajo está cerrada por sistemas, imposible su {operation}. Coordinar con Créditos.");
        }
    }

    /// <summary>
    /// PB (dw_2 doubleclicked): si la OT proviene de una Cotización, sus líneas no se modifican individualmente,
    /// salvo cotizaciones de Pretensados (PT), líneas PT o el tipo Descuento. Las líneas de productos de Hoja de
    /// Corte tampoco, mientras la OT tenga Hojas de Corte vigentes.
    /// </summary>
    public static void EnsureLineEditable(OrdenTrabajoVenta ot, OrdenTrabajoVentaDetalle line, bool hasHojasCorte)
    {
        if (ot.CotizacionCode is not null
            && ot.CotizacionNegocioCode != NegocioPretensados
            && line.NegocioCode != NegocioPretensados
            && ot.Type != TipoOrdenTrabajoVenta.Descuento)
        {
            throw new InvalidOperationException(
                "Imposible su modificación: la Orden de Trabajo proviene de una Cotización.");
        }

        if (hasHojasCorte && IsHojaCorteProducto(line.ProductoTipoCode))
        {
            throw new InvalidOperationException(
                "Imposible su modificación: la línea proviene de una Hoja de Corte. " +
                "Para cambiarla anule la OT (libera las Hojas de Corte).");
        }
    }

    /// <summary>true si la OT tiene al menos una Hoja de Corte vigente (no anulada) asociada.</summary>
    public static Task<bool> HasHojasCorteAsync(IUnitOfWork uow, string otCode, CancellationToken ct)
        => uow.Facturacion.Transacciones.HojasCorte
            .Query()
            .AsNoTracking()
            .AnyAsync(h => h.OrdenTrabajoCode == otCode && h.Status != EstadoHojaCorte.Anulado, ct);

    // ---------------------------------------------------------------- montos

    /// <summary>
    /// Total de línea = ROUND(cantidad * precio, 2). En productos tipo 23 el precio ya viene negativo.
    /// </summary>
    public static decimal LineTotal(decimal quantity, decimal unitPrice)
        => Math.Round(quantity * unitPrice, 2, MidpointRounding.AwayFromZero);

    /// <summary>Precio con el signo del producto: tipo 23 (descuento) se guarda negativo.</summary>
    public static decimal SignedPrice(string productoTipoCode, decimal unitPrice)
        => productoTipoCode == DescuentoProductoTipo ? -Math.Abs(unitPrice) : unitPrice;

    /// <summary>Total de la OT: suma de las líneas no anuladas.</summary>
    public static decimal SumTotal(IEnumerable<OrdenTrabajoVentaDetalle> lines)
        => Math.Round(
            lines.Where(l => l.Status != EstadoDetalleOrdenTrabajoVenta.Anulado).Sum(l => l.Total),
            2,
            MidpointRounding.AwayFromZero);

    public static void Recalculate(OrdenTrabajoVenta ot) => ot.Total = SumTotal(ot.Detalles);

    /// <summary>Tasa de IGV vigente (Comunes.ControlVentas); 18 si no hay configuración.</summary>
    public static async Task<decimal> GetIgvRateAsync(IUnitOfWork uow, CancellationToken ct)
    {
        var rate = await uow.Comunes.ControlesVentas
            .Query()
            .AsNoTracking()
            .Select(x => (decimal?)x.IgvPercentage)
            .FirstOrDefaultAsync(ct);

        return rate ?? 18m;
    }

    public static decimal WithIgv(decimal total, decimal igvRate)
        => Math.Round(total * (1m + igvRate / 100m), 2, MidpointRounding.AwayFromZero);

    // ---------------------------------------------------------------- tipo de productos

    /// <summary>
    /// Tipo de productos (PB: Tipo_prd / ue_cargartipo). Piloto del negocio: CO=0 AG=1 BL=2 BA=3 PT=4 AD=5 y letra
    /// C, A, B, E, V, Q. Se arma en el orden en que aparece cada negocio en las líneas, sin repetir.
    /// </summary>
    public static string ComputeProductTypes(IEnumerable<string> negocioCodesInOrder)
    {
        var letters = new List<char>();

        foreach (var negocio in negocioCodesInOrder)
        {
            var letter = negocio?.Trim().ToUpperInvariant() switch
            {
                "CO" => 'C',
                "AG" => 'A',
                "BL" => 'B',
                "BA" => 'E',
                "PT" => 'V',
                "AD" => 'Q',
                _ => '\0'
            };

            if (letter != '\0' && !letters.Contains(letter))
            {
                letters.Add(letter);
            }
        }

        return new string(letters.ToArray());
    }

    public static string ComputeProductTypes(OrdenTrabajoVenta ot)
        => ComputeProductTypes(
            ot.Detalles
                .Where(d => d.Status != EstadoDetalleOrdenTrabajoVenta.Anulado)
                .OrderBy(d => d.Item)
                .Select(d => d.NegocioCode));

    // ---------------------------------------------------------------- cotización

    public static bool HasCotizacion(OrdenTrabajoVenta ot) => ot.CotizacionCode is not null;

    // ---------------------------------------------------------------- letras del listado

    public static string StatusLetter(EstadoOrdenTrabajoVenta status) => status switch
    {
        EstadoOrdenTrabajoVenta.Activo => "A",
        EstadoOrdenTrabajoVenta.Inactivo => "I",
        EstadoOrdenTrabajoVenta.Cerrado => "C",
        EstadoOrdenTrabajoVenta.CerradoDefinitivo => "T",
        _ => "I"
    };

    public static string TypeLetter(TipoOrdenTrabajoVenta type, string? originCode) => type switch
    {
        TipoOrdenTrabajoVenta.Normal => "N",
        TipoOrdenTrabajoVenta.Descuento => "D",
        TipoOrdenTrabajoVenta.Anticipo => "A",
        _ => originCode is null ? "N" : "S"
    };

    // ---------------------------------------------------------------- parseo de filtros

    /// <summary>Acepta el código legacy (05, 06, 11, 42) o el nombre del enum.</summary>
    public static bool TryParseEstado(string? value, out EstadoOrdenTrabajoVenta estado)
    {
        switch (value?.Trim())
        {
            case "05": estado = EstadoOrdenTrabajoVenta.Activo; return true;
            case "06": estado = EstadoOrdenTrabajoVenta.Inactivo; return true;
            case "11": estado = EstadoOrdenTrabajoVenta.Cerrado; return true;
            case "42": estado = EstadoOrdenTrabajoVenta.CerradoDefinitivo; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out estado) && System.Enum.IsDefined(estado);
        }
    }

    /// <summary>Acepta la letra legacy (P, A, C, G) o el nombre del enum.</summary>
    public static bool TryParseEtapa(string? value, out EtapaOrdenTrabajoVenta etapa)
    {
        switch (value?.Trim().ToUpperInvariant())
        {
            case "P": etapa = EtapaOrdenTrabajoVenta.Pendiente; return true;
            case "A": etapa = EtapaOrdenTrabajoVenta.Anulada; return true;
            case "C": etapa = EtapaOrdenTrabajoVenta.AprobadaCredito; return true;
            case "G": etapa = EtapaOrdenTrabajoVenta.GeneraProgramacion; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out etapa) && System.Enum.IsDefined(etapa);
        }
    }

    /// <summary>Acepta la letra legacy (N, S, A, D, C) o el nombre del enum.</summary>
    public static bool TryParseTipo(string? value, out TipoOrdenTrabajoVenta tipo)
    {
        switch (value?.Trim().ToUpperInvariant())
        {
            case "N": tipo = TipoOrdenTrabajoVenta.Normal; return true;
            case "S": tipo = TipoOrdenTrabajoVenta.Saldo; return true;
            case "A": tipo = TipoOrdenTrabajoVenta.Anticipo; return true;
            case "D": tipo = TipoOrdenTrabajoVenta.Descuento; return true;
            case "C": tipo = TipoOrdenTrabajoVenta.Consolidacion; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out tipo) && System.Enum.IsDefined(tipo);
        }
    }

    public static bool IsAll(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);
}
