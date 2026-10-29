// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/Common/OrdenCompraAdministracionTotalsCalculator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common
{
    /// <summary>
    /// Misma fórmula que OrdenCompraTotalsCalculator (Logística), aplicada al detalle de la
    /// Orden de Compra de Administración:
    ///
    ///   Suma         = Σ Detalle.Total
    ///   Igv          = ComprobantePago.AffectsIgv      ? round(Suma × IgvPercentage / 100, 2)         : 0
    ///   RentaCompra  = ComprobantePago.AffectsIncomeTax ? round(Suma × WithholdingPercentage / 100, 2) : 0
    ///   FonaviCompra = ComprobantePago.AffectsFonavi    ? round(Suma × FonaviPercentage / 100, 2)      : 0
    ///   Si AffectsIncomeTax Y Suma &lt;= WithholdingCap -> RentaCompra = 0, FonaviCompra = 0
    ///   NoGravable/Servicio/IgvExterior: digitados, nunca se tocan aquí.
    ///   NetoCompra   = Suma
    ///   TotalCompra  = Suma + Igv + NoGravable − Renta − Fonavi + Servicio + IgvExterior
    /// </summary>
    public static class OrdenCompraAdmTotalsCalculator
    {
        public static async Task RecalculateAsync(
            IUnitOfWork uow, OrdenCompraAdministracion orden, CancellationToken cancellationToken)
        {
            var suma = Math.Round(orden.Detalles.Sum(d => d.Total), 2, MidpointRounding.AwayFromZero);

            var control = await uow.Comunes.ControlesVentas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            var comprobante = !string.IsNullOrWhiteSpace(orden.ComprobantePagoCode)
                ? await uow.Comunes.ComprobantesPago.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Code == orden.ComprobantePagoCode, cancellationToken)
                : null;

            var afectaIgv = comprobante?.AffectsIgv ?? false;
            var igvPercentage = afectaIgv
                ? await PeriodoIgvRules.GetPorcentajeAsync(uow, orden.FechaEntrega, cancellationToken)
                : 0m;
            var withholdingPercentage = control?.WithholdingPercentage ?? 0;
            var fonaviPercentage = control?.FonaviPercentage ?? 0;
            var withholdingCap = control?.WithholdingCap ?? 0;

            var igv = afectaIgv
                ? Math.Round(suma * igvPercentage / 100m, 2, MidpointRounding.AwayFromZero)
                : 0;

            var afectaRenta = comprobante?.AffectsIncomeTax ?? false;
            var rentaSol = afectaRenta
                ? Math.Round(suma * withholdingPercentage / 100m, 2, MidpointRounding.AwayFromZero)
                : 0;
            var fonaviSol = (comprobante?.AffectsFonavi ?? false)
                ? Math.Round(suma * fonaviPercentage / 100m, 2, MidpointRounding.AwayFromZero)
                : 0;

            if (afectaRenta && suma <= withholdingCap)
            {
                rentaSol = 0;
                fonaviSol = 0;
            }

            orden.NetoCompra = suma;
            orden.IgvCompra = igv;
            orden.RentaCompra = rentaSol;
            orden.FonaviCompra = fonaviSol;
            orden.TotalCompra = suma + igv + orden.NoGravableCompra - rentaSol - fonaviSol
                                + orden.ServicioCompra + orden.IgvExteriorCompra;
        }

        public static decimal CalculateLineTotal(decimal cantidad, decimal precio, decimal descuento)
            => Math.Round(cantidad * precio - descuento, 2, MidpointRounding.AwayFromZero);
    }
}
