// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/Common/ImportacionNotaIngresoTotals.cs
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common
{
    /// <summary>
    /// Totales de la Nota de Ingreso generada desde una Importación (en soles).
    ///
    /// Legacy (ue_set_btn_grb_generar_ni): Igv = Igv_Imp × T_cambio + IGV de gastos (Logi_sp_Consulta_IgvGa_IgvExt)
    /// e IgvExterior = IGV exterior de gastos, ambos de TODA la importación. Si se genera por proveedor, ese
    /// criterio cargaría el IGV completo en cada nota. Acá el IGV de la nota se arma con lo de SUS líneas:
    ///   IGV líneas   = Σ IgvDet × TipoCambio de la importación
    ///   IGV gastos   = Σ IgvGasto de los artículos de la nota, por gasto: USD × TipoCambio del gasto, PEN × 1
    ///   IGV exterior = ídem con IgvExtGasto
    /// Al generar todas las líneas a la vez coincide con el legacy salvo redondeos.
    /// </summary>
    public static class ImportacionNotaIngresoTotals
    {
        public sealed record Result(decimal Total, decimal Igv, decimal IgvExterior, decimal Monto);

        public static Result Compute(Importacion importacion, IReadOnlyCollection<ImportacionDetalle> lineas, IReadOnlyCollection<decimal> totalesLinea)
        {
            var articulos = lineas.Select(l => l.ArticuloCode).ToHashSet();

            var igvLineas = lineas.Sum(l => l.IgvDet) * importacion.TipoCambio;

            decimal igvGastos = 0, igvExterior = 0;
            foreach (var gasto in importacion.Gastos)
            {
                var filas = gasto.Articulos.Where(a => articulos.Contains(a.ArticuloCode)).ToList();
                var factor = gasto.MonedaCode == "USD" ? gasto.TipoCambio : 1m;
                igvGastos += Math.Round(filas.Sum(a => a.IgvGasto) * factor, 4, MidpointRounding.AwayFromZero);
                igvExterior += Math.Round(filas.Sum(a => a.IgvExtGasto) * factor, 4, MidpointRounding.AwayFromZero);
            }

            var total = totalesLinea.Sum();
            var igv = Math.Round(igvLineas + igvGastos, 2, MidpointRounding.AwayFromZero);
            var igvExt = Math.Round(igvExterior, 2, MidpointRounding.AwayFromZero);

            // Legacy (caso general): Monto = Total + Igv + NoGravable + Renta + Fonavi + Servicio + IgvExt (los otros conceptos = 0).
            return new Result(total, igv, igvExt, total + igv + igvExt);
        }
    }
}
