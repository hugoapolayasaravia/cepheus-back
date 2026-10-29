// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/Common/ImportacionGastoCalculator.cs
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common
{
    /// <summary>
    /// Traducción de w_importaciones.tab_1.ue_suma (gastos administrativos):
    ///   igv   = round(NetoGasto × IGV% / 100, 4)
    ///   Si el comprobante AffectsForeignIgv (legacy: tipo '91'): Igv = 0 e IgvExterior = igv.
    ///   Si no: Igv = igv (o el valor digitado) e IgvExterior = 0.
    ///   Total = NetoGasto + NetoGastoInafecto + Igv + IgvExterior
    /// </summary>
    public static class ImportacionGastoCalculator
    {
        public static void Apply(ImportacionGasto gasto, ComprobantePago comprobante, decimal igvPercentage, decimal? igvManual)
        {
            var igvCalculado = Math.Round(gasto.NetoGasto * igvPercentage / 100m, 4, MidpointRounding.AwayFromZero);

            if (comprobante.AffectsForeignIgv)
            {
                gasto.Igv = 0;
                gasto.IgvExterior = igvCalculado;
            }
            else
            {
                gasto.Igv = igvManual ?? igvCalculado;
                gasto.IgvExterior = 0;
            }

            gasto.Total = gasto.NetoGasto + gasto.NetoGastoInafecto + gasto.Igv + gasto.IgvExterior;
        }
    }
}
