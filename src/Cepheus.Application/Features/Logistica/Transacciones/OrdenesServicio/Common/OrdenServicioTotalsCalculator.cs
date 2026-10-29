using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;

/// <summary>
/// Traducción de ue_actualiza_notaingreso del PowerBuilder para Orden de Servicio (misma rutina que usa la
/// Nota de Ingreso, ver NotaIngresoTotalsCalculator).
///   Total = suma de las líneas.
///   Monto = importe final, con fórmula distinta según el comprobante sea Recibo por Honorarios (02) u otro.
/// Las diferencias de signo entre ramas (p. ej. NoGravable) son las del legacy, a propósito.
/// El porcentaje de IGV sale de ControlVentas (el legacy usaba fg_igv por fecha).
/// </summary>
public static class OrdenServicioTotalsCalculator
{
    /// <summary>Alta: IGV / renta / fonavi se calculan por los flags del comprobante; el resto en 0.</summary>
    public static async Task ApplyOnCreateAsync(
        IUnitOfWork uow, OrdenServicio os, ComprobantePago comprobante, CancellationToken ct)
    {
        var control = await uow.Comunes.ControlesVentas.Query().AsNoTracking().FirstOrDefaultAsync(ct);
        var igvPct = comprobante.AffectsIgv
            ? await PeriodoIgvRules.GetPorcentajeAsync(uow, os.FechaEmision, ct)
            : 0m;
        var suma = Sum(os);

        var igv = ComputedIgv(comprobante, igvPct, suma);
        var renta = ComputedRenta(comprobante, control, suma);
        var fonavi = ComputedFonavi(comprobante, control, suma);

        decimal monto;
        if (comprobante.Code == NotaIngresoRules.ComprobanteHonorarios)
        {
            if (suma <= (control?.WithholdingCap ?? 0))
            {
                renta = 0;
                fonavi = 0;
            }
            monto = suma + igv - renta - fonavi;
        }
        else
        {
            monto = suma + igv + renta + fonavi;
        }

        os.Total = suma;
        os.Igv = igv;
        os.NoGravable = 0;
        os.Renta = renta;
        os.Fonavi = fonavi;
        os.Servicio = 0;
        os.IgvExterior = 0;
        os.Monto = monto;
    }

    /// <summary>
    /// Modificación de cabecera: los importes habilitados por el comprobante vienen del usuario; los no
    /// habilitados quedan en 0. Si el comprobante cambió a Honorarios se recalculan por flags.
    /// </summary>
    public static async Task ApplyOnUpdateAsync(
        IUnitOfWork uow,
        OrdenServicio os,
        ComprobantePago comprobante,
        bool comprobanteChanged,
        decimal igvInput,
        decimal noGravableInput,
        decimal rentaInput,
        decimal fonaviInput,
        decimal servicioInput,
        decimal igvExteriorInput,
        CancellationToken ct)
    {
        var control = await uow.Comunes.ControlesVentas.Query().AsNoTracking().FirstOrDefaultAsync(ct);
        var igvPct = comprobante.AffectsIgv
            ? await PeriodoIgvRules.GetPorcentajeAsync(uow, os.FechaEmision, ct)
            : 0m;
        var suma = Sum(os);

        decimal igv, noGravable, renta, fonavi, servicio, igvExterior, monto;

        if (comprobante.Code == NotaIngresoRules.ComprobanteHonorarios && comprobanteChanged)
        {
            igv = ComputedIgv(comprobante, igvPct, suma);
            renta = ComputedRenta(comprobante, control, suma);
            fonavi = ComputedFonavi(comprobante, control, suma);
            noGravable = 0;
            servicio = 0;
            igvExterior = 0;

            if (suma <= (control?.WithholdingCap ?? 0))
            {
                renta = 0;
                fonavi = 0;
            }

            monto = suma + igv - noGravable - renta - fonavi + servicio + igvExterior;
        }
        else
        {
            igv = comprobante.AffectsIgv ? igvInput : 0;
            noGravable = comprobante.IsNonTaxable ? noGravableInput : 0;
            renta = comprobante.AffectsIncomeTax ? rentaInput : 0;
            fonavi = comprobante.AffectsFonavi ? fonaviInput : 0;
            servicio = comprobante.IsService ? servicioInput : 0;
            igvExterior = comprobante.AffectsForeignIgv ? igvExteriorInput : 0;

            monto = comprobante.Code == NotaIngresoRules.ComprobanteHonorarios
                ? suma + igv - noGravable - renta - fonavi + servicio + igvExterior
                : suma + igv + renta + fonavi + servicio + igvExterior;
        }

        os.Total = suma;
        os.Igv = igv;
        os.NoGravable = noGravable;
        os.Renta = renta;
        os.Fonavi = fonavi;
        os.Servicio = servicio;
        os.IgvExterior = igvExterior;
        os.Monto = monto;
    }

    /// <summary>
    /// Cambio en las líneas (agregar / modificar / eliminar): recalcula IGV, renta y fonavi por los flags del
    /// comprobante y CONSERVA los importes manuales (no gravable, servicio, IGV exterior). El legacy los
    /// ponía en 0 en este caso; se conservan para no perder lo que digitó el usuario.
    /// </summary>
    public static async Task RecalculateAsync(
        IUnitOfWork uow, OrdenServicio os, ComprobantePago comprobante, CancellationToken ct)
    {
        var control = await uow.Comunes.ControlesVentas.Query().AsNoTracking().FirstOrDefaultAsync(ct);
        var igvPct = comprobante.AffectsIgv
            ? await PeriodoIgvRules.GetPorcentajeAsync(uow, os.FechaEmision, ct)
            : 0m;
        var suma = Sum(os);

        var igv = ComputedIgv(comprobante, igvPct, suma);
        var renta = ComputedRenta(comprobante, control, suma);
        var fonavi = ComputedFonavi(comprobante, control, suma);

        var esHonorarios = comprobante.Code == NotaIngresoRules.ComprobanteHonorarios;
        if (esHonorarios && suma <= (control?.WithholdingCap ?? 0))
        {
            renta = 0;
            fonavi = 0;
        }

        os.Total = suma;
        os.Igv = igv;
        os.Renta = renta;
        os.Fonavi = fonavi;
        os.Monto = esHonorarios
            ? suma + igv - os.NoGravable - renta - fonavi + os.Servicio + os.IgvExterior
            : suma + igv + renta + fonavi + os.Servicio + os.IgvExterior;
    }

    private static decimal Sum(OrdenServicio os)
        => Math.Round(
            os.Detalles.Where(d => d.Estado != Cepheus.Domain.Logistica.Enum.EstadoOrdenServicioDetalle.Anulado)
                       .Sum(d => d.Total),
            2, MidpointRounding.AwayFromZero);

    // El porcentaje sale de PeriodoIgv según la fecha de emisión (fg_igv(fecha) del legacy).
    private static decimal ComputedIgv(ComprobantePago c, decimal igvPercentage, decimal suma)
        => c.AffectsIgv
            ? Math.Round(suma * igvPercentage / 100m, 2, MidpointRounding.AwayFromZero)
            : 0;

    private static decimal ComputedRenta(ComprobantePago c, ControlVentas? control, decimal suma)
        => c.AffectsIncomeTax
            ? Math.Round(suma * (control?.WithholdingPercentage ?? 0) / 100m, 2, MidpointRounding.AwayFromZero)
            : 0;

    private static decimal ComputedFonavi(ComprobantePago c, ControlVentas? control, decimal suma)
        => c.AffectsFonavi
            ? Math.Round(suma * (control?.FonaviPercentage ?? 0) / 100m, 2, MidpointRounding.AwayFromZero)
            : 0;
}
