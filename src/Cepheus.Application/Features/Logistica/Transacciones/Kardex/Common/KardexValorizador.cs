using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Transacciones.Kardex.Common;

/// <summary>Movimiento de entrada al cálculo (una fila de MovimientosArticulo, sin dependencias de EF).</summary>
public sealed record KardexEntrada(
    long Id,
    DateTime Fecha,
    string PeriodCode,
    TipoMovimientoArticulo Tipo,
    OrigenMovimientoArticulo Origen,
    decimal Cantidad,
    decimal CostoUnitario,
    bool EsReversion,
    long? ReversaDeMovimientoId);

/// <summary>Resultado del cálculo para un movimiento, en orden cronológico.</summary>
public sealed record KardexLinea(
    KardexEntrada Entrada,
    // Movimiento anulado (él y su reversión): se muestra pero no altera saldo ni costo.
    bool Anulado,
    // Costo unitario valorizado: el del ingreso, o el promedio vigente en una salida.
    decimal CostoValorizado,
    decimal SaldoCantidad,
    decimal CostoPromedio,
    decimal SaldoImporte)
{
    public bool StockNegativo => SaldoCantidad < 0;
}

/// <summary>
/// Valorización por costo promedio móvil, en orden cronológico. Es la regla única del Kardex y del cierre
/// mensual: ambos deben llamar a esta clase para que sus números coincidan.
///
/// Traduce el cursor C_CIERRE de Logi_sp_Actualiza_Mes_Consumos (ORDER BY artículo, fecha, tipo 'I' antes de 'S'):
/// - Ingreso: promedio = ROUND((Stock*Promedio + Cant*Costo) / (Stock + Cant), 6) si el saldo resultante es > 0;
///   si el saldo queda <= 0 el promedio pasa a ser el costo del ingreso.
/// - Nota de Crédito por devolución (motivo 02): se trata como ingreso de cantidad NEGATIVA al costo de la nota
///   (el legacy la procesa dentro de los ingresos). Si el saldo queda <= 0 el promedio no cambia.
/// - Salida: sale al promedio vigente; no altera el promedio. El stock puede quedar negativo
///   (el legacy lo registraba en ERRORES; aquí la línea trae StockNegativo = true).
/// - Movimiento anulado: se ignora junto con su reversión.
///
/// - Ajuste de precio (Nota de Crédito motivo 01, Origen = AjustePrecio): no mueve stock; el promedio pasa a ser el
///   promedio del mes que el cierre calcula con CierreMensualCalculator. El Kardex de consulta no lo incluye
///   (solo avisa), el cierre sí.
/// </summary>
public static class KardexValorizador
{
    public const int DecimalesCosto = 6;

    public static IReadOnlyList<KardexLinea> Calcular(
        decimal saldoInicialCantidad,
        decimal saldoInicialCosto,
        IEnumerable<KardexEntrada> movimientos)
    {
        var lista = movimientos.ToList();
        var anulados = IdsAnulados(lista);

        var ordenados = lista
            .OrderBy(m => m.Fecha.Date)
            .ThenBy(GrupoDelDia)
            .ThenBy(m => m.Id)
            .ToList();

        var stock = saldoInicialCantidad;
        var promedio = saldoInicialCosto;
        var resultado = new List<KardexLinea>(ordenados.Count);

        foreach (var m in ordenados)
        {
            if (anulados.Contains(m.Id))
            {
                resultado.Add(new KardexLinea(m, true, m.CostoUnitario, stock, promedio, Importe(stock, promedio)));
                continue;
            }

            decimal costoValorizado;

            if (m.Origen == OrigenMovimientoArticulo.AjustePrecio)
            {
                // Nota de Crédito de ajuste: no mueve stock; fija el promedio (CostoUnitario ya trae el promedio del mes).
                promedio = m.CostoUnitario;
                costoValorizado = promedio;
            }
            else if (m.Origen == OrigenMovimientoArticulo.NotaCredito)
            {
                // Devolución: ingreso negativo al costo de la nota.
                var saldo = stock - m.Cantidad;
                if (saldo > 0)
                {
                    promedio = Math.Round(
                        ((stock * promedio) - (m.Cantidad * m.CostoUnitario)) / saldo,
                        DecimalesCosto, MidpointRounding.AwayFromZero);
                }

                stock = saldo;
                costoValorizado = m.CostoUnitario;
            }
            else if (m.Tipo == TipoMovimientoArticulo.Ingreso)
            {
                var saldo = stock + m.Cantidad;
                promedio = saldo > 0
                    ? Math.Round(
                        ((stock * promedio) + (m.Cantidad * m.CostoUnitario)) / saldo,
                        DecimalesCosto, MidpointRounding.AwayFromZero)
                    : m.CostoUnitario;

                stock = saldo;
                costoValorizado = m.CostoUnitario;
            }
            else
            {
                stock -= m.Cantidad;
                costoValorizado = promedio;
            }

            resultado.Add(new KardexLinea(m, false, costoValorizado, stock, promedio, Importe(stock, promedio)));
        }

        return resultado;
    }

    /// <summary>Ids de los movimientos anulados: el original y su reversión (la reversión apunta al original).</summary>
    public static HashSet<long> IdsAnulados(IEnumerable<KardexEntrada> movimientos)
    {
        var anulados = new HashSet<long>();

        foreach (var m in movimientos.Where(m => m.EsReversion && m.ReversaDeMovimientoId.HasValue))
        {
            anulados.Add(m.Id);
            anulados.Add(m.ReversaDeMovimientoId!.Value);
        }

        return anulados;
    }

    public static decimal Importe(decimal cantidad, decimal costo) =>
        Math.Round(cantidad * costo, 2, MidpointRounding.AwayFromZero);

    // Orden dentro de un mismo día: apertura, ingresos (incluida la devolución, que el legacy procesa como
    // ingreso) y al final las salidas. Reproduce ORDER BY fec, TIPO_MOV del legacy.
    private static int GrupoDelDia(KardexEntrada m)
    {
        if (m.Origen == OrigenMovimientoArticulo.Apertura) return 0;
        if (m.Origen == OrigenMovimientoArticulo.NotaCredito) return 1;
        return m.Tipo == TipoMovimientoArticulo.Ingreso ? 1 : 2;
    }
}
