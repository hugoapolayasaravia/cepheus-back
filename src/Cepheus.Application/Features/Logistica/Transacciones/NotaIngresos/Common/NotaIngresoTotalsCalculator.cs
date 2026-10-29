using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;

/// <summary>
/// Traducción literal de ue_actualiza_notaingreso del PowerBuilder.
///   Total = suma de las líneas.
///   Monto = importe final, con fórmulas distintas según el comprobante sea Recibo por
///           Honorarios (02) u otro, y según se esté creando o modificando.
/// Las diferencias de signo entre ramas (p. ej. NoGravable) son las del legacy, a propósito.
/// </summary>
public static class NotaIngresoTotalsCalculator
{
    /// <summary>Alta: IGV / renta / fonavi se calculan por los flags del comprobante; el resto en 0.</summary>
    public static async Task ApplyOnCreateAsync(
        IUnitOfWork uow, NotaIngreso note, ComprobantePago comprobante, CancellationToken ct)
    {
        var control = await uow.Comunes.ControlesVentas.Query().AsNoTracking().FirstOrDefaultAsync(ct);
        var igvPct = comprobante.AffectsIgv
            ? await PeriodoIgvRules.GetPorcentajeAsync(uow, note.FechaEmision, ct)
            : 0m;
        var suma = Sum(note);

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

        note.Total = suma;
        note.Igv = igv;
        note.NoGravable = 0;
        note.Renta = renta;
        note.Fonavi = fonavi;
        note.Servicio = 0;
        note.IgvExterior = 0;
        note.Monto = monto;
    }

    /// <summary>
    /// Modificación: los importes habilitados por el comprobante vienen del usuario; los no
    /// habilitados quedan en 0. Si el comprobante cambió a/desde Honorarios se recalculan por flags.
    /// </summary>
    public static async Task ApplyOnUpdateAsync(
        IUnitOfWork uow,
        NotaIngreso note,
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
            ? await PeriodoIgvRules.GetPorcentajeAsync(uow, note.FechaEmision, ct)
            : 0m;
        var suma = Sum(note);

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

        note.Total = suma;
        note.Igv = igv;
        note.NoGravable = noGravable;
        note.Renta = renta;
        note.Fonavi = fonavi;
        note.Servicio = servicio;
        note.IgvExterior = igvExterior;
        note.Monto = monto;
    }

    private static decimal Sum(NotaIngreso note)
        => Math.Round(note.Detalles.Sum(d => d.Total), 2, MidpointRounding.AwayFromZero);

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
