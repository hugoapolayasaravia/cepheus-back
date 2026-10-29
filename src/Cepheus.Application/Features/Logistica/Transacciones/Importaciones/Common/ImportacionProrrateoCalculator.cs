// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/Common/ImportacionProrrateoCalculator.cs
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common
{
    /// <summary>
    /// Traducción de Logi_sp_Actualiza_Importacion (sin cursores).
    /// Requiere <c>Importacion.Detalles</c> (con <c>Articulo</c>) y <c>Importacion.Gastos</c> (con <c>Articulos</c>) cargados.
    ///
    /// Paso 1 — distribución a cada línea:
    ///   ratio(sf)   = round(FOB de la subfamilia / total FOB, 5)       (total FOB redondeado a 2 decimales, como @t_valor_fob DECIMAL(12,2))
    ///   share(línea)= ValorAduana de la línea / ValorAduana de su subfamilia   (FLOAT en el legacy)
    ///   AdvaloremDet  = round(round(Advalorem × ratio, 4) × share, 4)  (igual Sobretasa y OtrosGastos)
    ///   IgvDet        = round(round((AduanaSF + AdvaloremSF + SobretasaSF) × IGV%, 4) × share, 4)
    ///   Cada gasto: ValorGasto = round(round((Neto + NetoInafecto) × ratio, 4) × share, 4); idem Igv e IgvExterior.
    ///
    /// Paso 2 — ValorDet (costo en soles de la línea, SIN IGV):
    ///   resultado(sf) = (FOB + Flete + Seguro + Advalorem + Sobretasa + OtrosGastos)(sf) × TipoCambio de la cabecera
    ///                   + gastos de la subfamilia en soles (USD × TipoCambio del gasto; PEN × 1)
    ///   ValorDet      = round(resultado(sf) × share, 4)
    ///
    /// Decisiones frente al legacy:
    ///   - Solo se escriben las líneas Pendiente: las ya Procesadas conservan su costo (el stock ya se movió con él).
    ///   - IGV% único (ControlVentas) en vez de fn_Igv(fecha de cada línea).
    ///   - Replica la PK legacy de MImportacionGDet_Articulos (sin proveedor de la línea): si el mismo artículo
    ///     viene de dos proveedores, la fila del gasto se sobrescribe con la última línea (upsert, igual que el SP).
    /// </summary>
    public static class ImportacionProrrateoCalculator
    {
        private const string Soles = "PEN";
        private const string Dolares = "USD";

        public static void Calculate(Importacion importacion, decimal igvPercentage)
        {
            var detalles = importacion.Detalles.ToList();
            if (detalles.Count == 0)
            {
                return;
            }

            var totalFob = Math.Round(detalles.Sum(d => d.ValorFob), 2, MidpointRounding.AwayFromZero);
            if (totalFob == 0)
            {
                throw new InvalidOperationException("No se puede prorratear: el total FOB de la Importación es cero.");
            }

            var familiaPorArticulo = detalles
                .GroupBy(d => d.ArticuloCode)
                .ToDictionary(g => g.Key, g => g.First().Articulo.SubFamiliaCode);

            var calculos = new List<LineaCalculada>();

            foreach (var familia in detalles.GroupBy(d => d.Articulo.SubFamiliaCode))
            {
                var fobFamilia = familia.Sum(d => d.ValorFob);
                var aduanaFamilia = familia.Sum(d => d.ValorAduana);
                var ratio = Math.Round(fobFamilia / totalFob, 5, MidpointRounding.AwayFromZero);

                var advFamilia = R4(importacion.Advalorem * ratio);
                var sobFamilia = R4(importacion.Sobretasa * ratio);
                var otrosFamilia = R4(importacion.OtrosGastos * ratio);
                var igvFamilia = R4((aduanaFamilia + advFamilia + sobFamilia) * (igvPercentage / 100m));

                foreach (var d in familia)
                {
                    var share = aduanaFamilia == 0 ? 0d : (double)d.ValorAduana / (double)aduanaFamilia;

                    var calculo = new LineaCalculada(d, familia.Key, share)
                    {
                        Advalorem = Prorratear(advFamilia, share),
                        Sobretasa = Prorratear(sobFamilia, share),
                        Igv = Prorratear(igvFamilia, share),
                        Otros = Prorratear(otrosFamilia, share)
                    };

                    if (d.Estado == EstadoImportacion.Pendiente)
                    {
                        foreach (var gasto in importacion.Gastos)
                        {
                            Upsert(gasto, d.ArticuloCode,
                                Prorratear(R4((gasto.NetoGasto + gasto.NetoGastoInafecto) * ratio), share),
                                Prorratear(R4(gasto.Igv * ratio), share),
                                Prorratear(R4(gasto.IgvExterior * ratio), share));
                        }
                    }

                    calculos.Add(calculo);
                }
            }

            // Paso 2: costo en soles por subfamilia (los gastos se leen de lo ya asignado a los artículos).
            var tc = importacion.TipoCambio;

            foreach (var familia in calculos.GroupBy(c => c.SubFamiliaCode))
            {
                var lineas = familia.Select(c => c.Detalle).ToList();

                var gastoSoles = importacion.Gastos
                    .GroupBy(g => (g.MonedaCode, g.TipoCambio))
                    .Sum(grupo =>
                    {
                        var valor = grupo
                            .SelectMany(g => g.Articulos)
                            .Where(a => familiaPorArticulo.TryGetValue(a.ArticuloCode, out var sf) && sf == familia.Key)
                            .Sum(a => a.ValorGasto);

                        return grupo.Key.MonedaCode switch
                        {
                            Soles => R4(valor),
                            Dolares => R4(valor * grupo.Key.TipoCambio),
                            _ => throw new InvalidOperationException(
                                $"Moneda '{grupo.Key.MonedaCode}' no soportada en gastos de importación (solo PEN y USD).")
                        };
                    });

                var resultado =
                    R4(lineas.Sum(d => d.ValorFob) * tc) +
                    R4(lineas.Sum(d => d.Flete) * tc) +
                    R4(lineas.Sum(d => d.Seguro) * tc) +
                    R4(familia.Sum(c => c.Advalorem) * tc) +
                    R4(familia.Sum(c => c.Sobretasa) * tc) +
                    gastoSoles +
                    R4(familia.Sum(c => c.Otros) * tc);

                foreach (var c in familia.Where(c => c.Detalle.Estado == EstadoImportacion.Pendiente))
                {
                    c.Detalle.PorcentajeDet = Math.Round((decimal)c.Share, 10, MidpointRounding.AwayFromZero);
                    c.Detalle.AdvaloremDet = c.Advalorem;
                    c.Detalle.SobretasaDet = c.Sobretasa;
                    c.Detalle.IgvDet = c.Igv;
                    c.Detalle.OtrosGastosDet = c.Otros;
                    c.Detalle.ValorDet = Prorratear(resultado, c.Share);
                }
            }
        }

        private static void Upsert(ImportacionGasto gasto, string articuloCode, decimal valor, decimal igv, decimal igvExt)
        {
            var fila = gasto.Articulos.FirstOrDefault(a => a.ArticuloCode == articuloCode);

            if (fila is null)
            {
                gasto.Articulos.Add(new ImportacionGastoArticulo
                {
                    PlantaCode = gasto.PlantaCode,
                    ImportacionCode = gasto.ImportacionCode,
                    ProveedorCode = gasto.ProveedorCode,
                    NumeroDocumento = gasto.NumeroDocumento,
                    ArticuloCode = articuloCode,
                    ValorGasto = valor,
                    IgvGasto = igv,
                    IgvExtGasto = igvExt
                });
                return;
            }

            fila.ValorGasto = valor;
            fila.IgvGasto = igv;
            fila.IgvExtGasto = igvExt;
        }

        /// <summary>Monto × share en FLOAT (como el legacy) y redondeo a 4 decimales.</summary>
        private static decimal Prorratear(decimal monto, double share)
            => R4((decimal)((double)monto * share));

        private static decimal R4(decimal value) => Math.Round(value, 4, MidpointRounding.AwayFromZero);

        private sealed class LineaCalculada
        {
            public LineaCalculada(ImportacionDetalle detalle, string subFamiliaCode, double share)
            {
                Detalle = detalle;
                SubFamiliaCode = subFamiliaCode;
                Share = share;
            }

            public ImportacionDetalle Detalle { get; }
            public string SubFamiliaCode { get; }
            public double Share { get; }
            public decimal Advalorem { get; set; }
            public decimal Sobretasa { get; set; }
            public decimal Igv { get; set; }
            public decimal Otros { get; set; }
        }
    }
}
