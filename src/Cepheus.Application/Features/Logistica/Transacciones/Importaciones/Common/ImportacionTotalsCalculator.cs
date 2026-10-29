// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/Common/ImportacionTotalsCalculator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common
{
    /// <summary>
    /// Traducción de la fórmula legacy (PowerBuilder, w_importaciones.ue_actualiza_importacion):
    ///
    ///   TotalFob/Flete/Seguro = Σ del detalle (ImportacionDetalle)
    ///   TotalAduana           = TotalFob + TotalFlete + TotalSeguro
    ///   Igv                   = round((TotalAduana + Advalorem + Sobretasa) × IGV% / 100, 2)
    ///   TipoCambio            = venta de TipoCambio en FechaPoliza (error si no existe)
    ///
    /// Advalorem, Sobretasa y OtrosGastos son digitados: nunca se derivan acá.
    /// IGV%: fg_igv(fecha_pol) del legacy; acá se toma el porcentaje del período de IGV (Comunes.PeriodoIgv)
    /// que contiene la fecha de póliza.
    /// </summary>
    public static class ImportacionTotalsCalculator
    {
        public static void Recalculate(
            Importacion importacion,
            decimal totalFob,
            decimal totalFlete,
            decimal totalSeguro,
            decimal igvPercentage)
        {
            importacion.TotalFob = Math.Round(totalFob, 2, MidpointRounding.AwayFromZero);
            importacion.TotalFlete = Math.Round(totalFlete, 2, MidpointRounding.AwayFromZero);
            importacion.TotalSeguro = Math.Round(totalSeguro, 2, MidpointRounding.AwayFromZero);
            importacion.TotalAduana = importacion.TotalFob + importacion.TotalFlete + importacion.TotalSeguro;

            var baseImponible = importacion.TotalAduana + importacion.Advalorem + importacion.Sobretasa;
            importacion.Igv = Math.Round(baseImponible * igvPercentage / 100m, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>Recalcula desde el detalle cargado en <c>importacion.Detalles</c> (debe venir con Include).</summary>
        public static void Recalculate(Importacion importacion, decimal igvPercentage)
            => Recalculate(
                importacion,
                importacion.Detalles.Sum(d => d.ValorFob),
                importacion.Detalles.Sum(d => d.Flete),
                importacion.Detalles.Sum(d => d.Seguro),
                igvPercentage);

        /// <summary>
        /// IGV% vigente en la fecha indicada (fg_igv(fecha) del legacy), desde Comunes.PeriodoIgv.
        /// Cabecera y líneas: fecha de póliza. Gastos: fecha de emisión del gasto.
        /// </summary>
        public static Task<decimal> GetIgvPercentageAsync(IUnitOfWork uow, DateTime fecha, CancellationToken cancellationToken)
            => PeriodoIgvRules.GetPorcentajeAsync(uow, fecha, cancellationToken);

        /// <summary>Tipo de cambio venta del día indicado; falla si no está registrado (mismo criterio legacy).</summary>
        public static async Task<decimal> ResolveTipoCambioAsync(IUnitOfWork uow, DateTime fecha, CancellationToken cancellationToken)
        {
            var day = DateOnly.FromDateTime(fecha);

            var tipoCambio = await uow.Comunes.TiposCambio.Query()
                .AsNoTracking()
                .Where(t => t.Date == day)
                .Select(t => (decimal?)t.SellRate)
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoCambio is null)
            {
                throw new InvalidOperationException(
                    $"No se registra Tipo de Cambio del día {day:dd/MM/yyyy} (fecha de póliza).");
            }

            return tipoCambio.Value;
        }
    }
}
