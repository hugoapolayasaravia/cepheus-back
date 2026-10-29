namespace Cepheus.Application.Features.Logistica.Transacciones.ReporteCierre.Common;

/// <summary>Saldo de un artículo en un período cerrado, con los datos del artículo (una fila de SaldosMensualesArticulo).</summary>
public sealed record SaldoLeido(
    string ArticuloCode,
    string ArticuloName,
    string UnidadMedidaCode,
    string FamiliaCode,
    string SubFamiliaCode,
    string PeriodCode,
    decimal Quantity,
    decimal AverageCost,
    decimal Amount);

public sealed record ReporteSaldo(decimal Cantidad, decimal CostoPromedio, decimal Importe);

public sealed record ReporteFila(
    string ArticuloCode,
    string ArticuloName,
    string UnidadMedidaCode,
    string FamiliaCode,
    string SubFamiliaCode,
    ReporteSaldo Comparacion,
    ReporteSaldo Actual,
    decimal DiferenciaCantidad,
    decimal DiferenciaImporte);

public sealed record ReporteTotales(
    int Articulos,
    decimal ImporteComparacion,
    decimal ImporteActual,
    decimal DiferenciaImporte);

/// <summary>
/// Arma el reporte de cierre comparando dos períodos. Lógica pura (sin base de datos).
/// Reemplaza a Logi_sp_Rpt_Cierre_x_Meses, que armaba el SELECT con las columnas de dos meses (D200306S, D200306P...)
/// como texto. Aquí cada período es una fila de SaldosMensualesArticulo y se compara cualquier par de períodos.
/// Un artículo sin fila en un período tiene saldo cero en él (no tuvo existencias ni movimientos).
/// </summary>
public static class ReporteCierreBuilder
{
    private static readonly ReporteSaldo SinSaldo = new(0m, 0m, 0m);

    public static (IReadOnlyList<ReporteFila> Filas, ReporteTotales Totales) Armar(
        IEnumerable<SaldoLeido> saldos,
        string periodoActual,
        string periodoComparacion,
        bool incluirSaldoCero)
    {
        var filas = new List<ReporteFila>();

        foreach (var grupo in saldos.GroupBy(s => s.ArticuloCode).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            var cualquiera = grupo.First();
            var actual = Convertir(grupo.FirstOrDefault(s => s.PeriodCode == periodoActual));
            var comparacion = Convertir(grupo.FirstOrDefault(s => s.PeriodCode == periodoComparacion));

            if (!incluirSaldoCero && actual.Cantidad == 0m && comparacion.Cantidad == 0m)
                continue;

            filas.Add(new ReporteFila(
                cualquiera.ArticuloCode,
                cualquiera.ArticuloName,
                cualquiera.UnidadMedidaCode,
                cualquiera.FamiliaCode,
                cualquiera.SubFamiliaCode,
                comparacion,
                actual,
                actual.Cantidad - comparacion.Cantidad,
                actual.Importe - comparacion.Importe));
        }

        var totales = new ReporteTotales(
            filas.Count,
            filas.Sum(f => f.Comparacion.Importe),
            filas.Sum(f => f.Actual.Importe),
            filas.Sum(f => f.DiferenciaImporte));

        return (filas, totales);
    }

    private static ReporteSaldo Convertir(SaldoLeido? s) =>
        s is null ? SinSaldo : new ReporteSaldo(s.Quantity, s.AverageCost, s.Amount);
}
