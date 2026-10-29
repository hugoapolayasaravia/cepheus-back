using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;

/// <summary>
/// Mantiene el vale de salida alineado con la Orden de Servicio. Traduce lo que el PowerBuilder hacía con
/// Logi_sp_AgregaMOrdenServicioDet_S (cada vez que se grababa / modificaba / eliminaba una línea de la orden se
/// reflejaba en el vale) y ue_actualiza_vales (importes del vale).
///
/// El vale guarda precios y totales en SOLES: si la orden es en dólares se convierten con el tipo de cambio
/// (el legacy calculaba el total ignorando el descuento en ese caso; aquí se convierte el total de la línea).
/// Nada de esto confirma cambios: los guarda el SaveChanges del caso de uso de la orden.
/// </summary>
public static class OrdenServicioSalidaSync
{
    public static decimal PrecioSoles(OrdenServicio os, OrdenServicioDetalle linea)
        => NotaIngresoRules.EsDolar(os.MonedaCode)
            ? Math.Round(linea.Precio * os.TipoCambio, 6, MidpointRounding.AwayFromZero)
            : linea.Precio;

    public static decimal TotalSoles(OrdenServicio os, OrdenServicioDetalle linea)
        => NotaIngresoRules.EsDolar(os.MonedaCode)
            ? Math.Round(linea.Total * os.TipoCambio, 2, MidpointRounding.AwayFromZero)
            : linea.Total;

    /// <summary>
    /// Arma el vale (con el mismo código que la orden) y sus líneas. La orden debe tener ya su Code y sus líneas.
    /// Los importes los calcula luego RecalculateAsync.
    /// </summary>
    public static OrdenServicioSalida CreateFor(OrdenServicio os, string trabajadorCode)
    {
        var vale = new OrdenServicioSalida
        {
            PlantaCode = os.PlantaCode,
            Code = os.Code,
            FechaEntrega = os.FechaRecepcion,
            TrabajadorCode = trabajadorCode,
            Estado = EstadoOrdenServicioSalida.Pendiente
        };

        SyncLines(null, os, vale);
        return vale;
    }

    /// <summary>Alinea las líneas del vale con las de la orden (alta / modificación / baja por ItemNumber).</summary>
    public static void SyncLines(IUnitOfWork? uow, OrdenServicio os) => SyncLines(uow, os, os.ValeSalida);

    private static void SyncLines(IUnitOfWork? uow, OrdenServicio os, OrdenServicioSalida vale)
    {
        vale.FechaEntrega = os.FechaRecepcion;

        foreach (var linea in os.Detalles.OrderBy(d => d.ItemNumber))
        {
            var salida = vale.Detalles.FirstOrDefault(d => d.ItemNumber == linea.ItemNumber);

            if (salida is null)
            {
                salida = new OrdenServicioSalidaDetalle
                {
                    PlantaCode = os.PlantaCode,
                    SalidaCode = vale.Code,
                    ItemNumber = linea.ItemNumber,
                    ArticuloCode = linea.ArticuloCode,
                    Estado = EstadoOrdenServicioSalidaDetalle.Pendiente
                };
                vale.Detalles.Add(salida);
            }

            salida.Glosa = linea.Glosa;
            salida.Cantidad = linea.Cantidad;
            salida.Precio = PrecioSoles(os, linea);
            salida.Total = TotalSoles(os, linea);
            salida.TipoValeCode = linea.TipoValeCode;
            salida.SubCentroCostoCode = linea.SubCentroCostoCode;
            salida.SubCentroEjecutorCode = linea.SubCentroEjecutorCode;
            salida.OrdenTrabajoCode = linea.OrdenTrabajoCode;
            salida.PlantaAfectadaCode = linea.PlantaAfectadaCode;
        }

        var items = os.Detalles.Select(d => d.ItemNumber).ToHashSet();
        foreach (var sobrante in vale.Detalles.Where(d => !items.Contains(d.ItemNumber)).ToList())
        {
            vale.Detalles.Remove(sobrante);
            uow?.Logistica.Transacciones.OrdenServicioSalidaDetalles.Remove(sobrante);
        }
    }

    /// <summary>
    /// ue_actualiza_vales: Neto = suma de las líneas vigentes (pendientes o procesadas), IGV con el porcentaje de
    /// ControlVentas, Total = Neto + IGV.
    /// </summary>
    public static async Task RecalculateAsync(IUnitOfWork uow, OrdenServicioSalida vale, CancellationToken ct)
    {
        var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(uow, vale.FechaEntrega, ct);

        var suma = Math.Round(
            vale.Detalles
                .Where(d => d.Estado is EstadoOrdenServicioSalidaDetalle.Pendiente
                                     or EstadoOrdenServicioSalidaDetalle.Procesado)
                .Sum(d => d.Total),
            2, MidpointRounding.AwayFromZero);

        vale.Neto = suma;
        vale.Igv = Math.Round(suma * igvPercentage / 100m, 2, MidpointRounding.AwayFromZero);
        vale.Total = vale.Neto + vale.Igv;
    }

    /// <summary>Procesar: el vale y sus líneas pendientes pasan a Procesado (Login_prs / Fecha_prs).</summary>
    public static void MarkProcessed(OrdenServicio os, string? usuario, DateTime fecha)
    {
        var vale = os.ValeSalida;

        foreach (var d in vale.Detalles.Where(d => d.Estado == EstadoOrdenServicioSalidaDetalle.Pendiente))
            d.Estado = EstadoOrdenServicioSalidaDetalle.Procesado;

        vale.Estado = EstadoOrdenServicioSalida.Procesado;
        vale.ProcesadoPor = usuario;
        vale.FechaProcesado = fecha;
    }

    /// <summary>
    /// Anular: las líneas procesadas pasan a Devuelto (14, como el legacy) y las pendientes a Anulado.
    /// </summary>
    public static void MarkAnulado(OrdenServicio os)
    {
        var vale = os.ValeSalida;

        foreach (var d in vale.Detalles)
        {
            d.Estado = d.Estado == EstadoOrdenServicioSalidaDetalle.Procesado
                ? EstadoOrdenServicioSalidaDetalle.Devuelto
                : EstadoOrdenServicioSalidaDetalle.Anulado;
        }

        vale.Estado = EstadoOrdenServicioSalida.Anulado;
    }
}
