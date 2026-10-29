using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common
{
    /// <summary>
    /// Misma fórmula que los totales de la Orden de Compra (flags de ComprobantePago + ControlVentas
    /// + PeriodoIgv), aplicada al Documento de Compra:
    ///
    ///   ParcialSol   = Σ Detalle.ParcialSol
    ///   IgvSol       = ComprobantePago.AffectsIgv      ? round(Parcial × IgvPercentage(FechaEmision) / 100, 2) : 0
    ///   RentaSol     = ComprobantePago.AffectsIncomeTax ? round(Parcial × WithholdingPercentage / 100, 2)       : 0
    ///   FonaviSol    = ComprobantePago.AffectsFonavi    ? round(Parcial × FonaviPercentage / 100, 2)            : 0
    ///   Si AffectsIncomeTax Y Parcial &lt;= WithholdingCap -> RentaSol = 0, FonaviSol = 0
    ///   NoGravableSol/ServicioSol/IgvExteriorSol: digitados, nunca se tocan aquí.
    ///   TotalSol     = Parcial + Igv + NoGravable − Renta − Fonavi + Servicio + IgvExterior
    /// </summary>
    public static class DocumentoCompraTotalsCalculator
    {
        public static async Task RecalculateAsync(
            IUnitOfWork uow, DocumentoCompra doc, CancellationToken cancellationToken)
        {
            var parcial = Math.Round(doc.Detalles.Sum(d => d.ParcialSol), 2, MidpointRounding.AwayFromZero);

            var control = await uow.Comunes.ControlesVentas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            var comprobante = await uow.Comunes.ComprobantesPago.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == doc.ComprobantePagoCode, cancellationToken);

            var afectaIgv = comprobante?.AffectsIgv ?? false;
            var igvPercentage = afectaIgv
                ? await PeriodoIgvRules.GetPorcentajeAsync(uow, doc.FechaEmision, cancellationToken)
                : 0m;
            var withholdingPercentage = control?.WithholdingPercentage ?? 0;
            var fonaviPercentage = control?.FonaviPercentage ?? 0;
            var withholdingCap = control?.WithholdingCap ?? 0;

            var igv = afectaIgv
                ? Math.Round(parcial * igvPercentage / 100m, 2, MidpointRounding.AwayFromZero)
                : 0;

            var afectaRenta = comprobante?.AffectsIncomeTax ?? false;
            var renta = afectaRenta
                ? Math.Round(parcial * withholdingPercentage / 100m, 2, MidpointRounding.AwayFromZero)
                : 0;
            var fonavi = (comprobante?.AffectsFonavi ?? false)
                ? Math.Round(parcial * fonaviPercentage / 100m, 2, MidpointRounding.AwayFromZero)
                : 0;

            if (afectaRenta && parcial <= withholdingCap)
            {
                renta = 0;
                fonavi = 0;
            }

            doc.ParcialSol = parcial;
            doc.IgvSol = igv;
            doc.RentaSol = renta;
            doc.FonaviSol = fonavi;
            doc.TotalSol = parcial + igv + doc.NoGravableSol - renta - fonavi
                           + doc.ServicioSol + doc.IgvExteriorSol;
        }

        public static decimal CalculateLineTotal(decimal cantidad, decimal precio)
            => Math.Round(cantidad * precio, 2, MidpointRounding.AwayFromZero);
    }
}
