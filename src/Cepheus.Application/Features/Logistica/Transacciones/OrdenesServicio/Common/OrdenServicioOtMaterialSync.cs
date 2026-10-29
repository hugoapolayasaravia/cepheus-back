using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Transacciones;
using Cepheus.Domain.Mantenimiento.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;

/// <summary>
/// Materiales de la Orden de Trabajo (OTRMaterial) que genera / revierte una Orden de Servicio.
///
/// ApplyAsync traduce Logi_sp_AgregaTOTRMateriales. Clave: (Planta, OT, Fecha, Artículo), con Fecha = fecha
/// de recepción de la orden. Por cada línea con OT, en orden de ítem:
///   - si no existe la fila: se inserta (cantidad, costo unitario, costo total, estado 13);
///   - si existe con estado 15 o 01: SOLO se cambia el estado a 13 (no se suma nada);
///   - si existe con otro estado: se SUMA cantidad y costo total, el costo unitario pasa a ser el de la
///     línea (no es promedio) y el estado queda en 13.
/// Las líneas se procesan una por una, de modo que dos líneas iguales de la misma orden se acumulan entre sí
/// (el SP legacy ya veía el estado 13 de la primera al llegar la segunda).
///
/// RevertAsync (anulación): el PowerBuilder pretendía borrar estas filas pero nunca lo hacía (variable sin
/// asignar). Aquí se RESTA la parte de esta orden y se elimina la fila si queda en cero. Pendiente de
/// confirmar qué significan los estados 15 y 01 para decidir si esos casos deben tratarse distinto.
///
/// El costo es siempre en SOLES. Todos los cambios se confirman con el SaveChanges del caso de uso.
/// </summary>
public static class OrdenServicioOtMaterialSync
{
    public const string EstadoProcesado = "13";

    private static bool EsEstadoPrevio(string? estado) => estado is "15" or "01";

    /// <summary>
    /// Costo de la línea en SOLES (el costo de la OT siempre es en soles; el PowerBuilder convertía con el
    /// tipo de cambio cuando la orden es en dólares).
    /// </summary>
    public static (decimal CostoUnitario, decimal CostoTotal) CostoEnSoles(OrdenServicio os, OrdenServicioDetalle linea)
    {
        if (!NotaIngresoRules.EsDolar(os.MonedaCode))
            return (linea.Precio, linea.Total);

        return (
            Math.Round(linea.Precio * os.TipoCambio, 6, MidpointRounding.AwayFromZero),
            Math.Round(linea.Total * os.TipoCambio, 2, MidpointRounding.AwayFromZero));
    }

    public static async Task ApplyAsync(
        IUnitOfWork uow, OrdenServicio os, IEnumerable<OrdenServicioDetalle> lineas, CancellationToken ct)
    {
        var fecha = os.FechaRecepcion.Date;

        // Filas ya leídas o creadas en esta operación (aún sin guardar), para que las líneas repetidas se acumulen.
        var cache = new Dictionary<(string Ot, string Articulo), OTRMaterial>();

        foreach (var linea in lineas.Where(l => l.OrdenTrabajoCode is not null).OrderBy(l => l.ItemNumber))
        {
            var key = (linea.OrdenTrabajoCode!, linea.ArticuloCode);
            var (costoUnitario, costoTotal) = CostoEnSoles(os, linea);

            if (!cache.TryGetValue(key, out var fila))
            {
                fila = await uow.Mantenimiento.Transacciones.OTRMateriales.Query()
                    .FirstOrDefaultAsync(m =>
                        m.PlantaCode == os.PlantaCode &&
                        m.OrdenTrabajoCode == key.Item1 &&
                        m.FechaProceso == fecha &&
                        m.ArticuloCode == key.Item2, ct);

                if (fila is not null)
                    cache[key] = fila;
            }

            if (fila is null)
            {
                fila = new OTRMaterial
                {
                    PlantaCode = os.PlantaCode,
                    OrdenTrabajoCode = key.Item1,
                    FechaProceso = fecha,
                    ArticuloCode = key.Item2,
                    Cantidad = linea.Cantidad,
                    CostoUnitario = Math.Round(costoUnitario, 5, MidpointRounding.AwayFromZero),
                    CostoTotal = costoTotal,
                    EstadoCode = EstadoProcesado
                };
                await uow.Mantenimiento.Transacciones.OTRMateriales.AddAsync(fila, ct);
                cache[key] = fila;
            }
            else if (EsEstadoPrevio(fila.EstadoCode))
            {
                // SP: con estado 15 o 01 solo se cambia el estado.
                fila.EstadoCode = EstadoProcesado;
            }
            else
            {
                fila.Cantidad += linea.Cantidad;
                fila.CostoUnitario = Math.Round(costoUnitario, 5, MidpointRounding.AwayFromZero);
                fila.CostoTotal += costoTotal;
                fila.EstadoCode = EstadoProcesado;
            }
        }
    }

    public static async Task RevertAsync(
        IUnitOfWork uow, OrdenServicio os, IEnumerable<OrdenServicioDetalle> lineas, CancellationToken ct)
    {
        var fecha = os.FechaRecepcion.Date;

        var grupos = lineas
            .Where(l => l.OrdenTrabajoCode is not null)
            .GroupBy(l => (Ot: l.OrdenTrabajoCode!, l.ArticuloCode));

        foreach (var g in grupos)
        {
            var existente = await uow.Mantenimiento.Transacciones.OTRMateriales.Query()
                .FirstOrDefaultAsync(m =>
                    m.PlantaCode == os.PlantaCode &&
                    m.OrdenTrabajoCode == g.Key.Ot &&
                    m.FechaProceso == fecha &&
                    m.ArticuloCode == g.Key.ArticuloCode, ct);

            if (existente is null)
                continue;

            existente.Cantidad -= g.Sum(l => l.Cantidad);
            existente.CostoTotal -= g.Sum(l => CostoEnSoles(os, l).CostoTotal);

            if (existente.Cantidad <= 0)
            {
                uow.Mantenimiento.Transacciones.OTRMateriales.Remove(existente);
            }
            else
            {
                existente.CostoUnitario = Math.Round(
                    existente.CostoTotal / existente.Cantidad, 5, MidpointRounding.AwayFromZero);
            }
        }
    }
}
