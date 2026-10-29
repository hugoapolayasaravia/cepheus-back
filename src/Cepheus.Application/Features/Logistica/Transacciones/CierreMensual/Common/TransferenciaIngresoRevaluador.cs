using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;

/// <summary>
/// Lleva al ingreso del almacén destino el costo final que el cierre fijó en la salida del origen
/// (Logi_sp_Actualiza_Transferencias del legacy): corrige precio y total de la línea de la Nota de Ingreso por
/// transferencia, sus totales de cabecera y el costo del movimiento en el Kardex del destino.
///
/// - Solo corrige si el período del destino sigue abierto. Si ya está cerrado deja una incidencia en el cierre del
///   origen y no toca nada (el legacy lo corregía igual, sin avisar).
/// - Cada cambio queda auditado en CierreRevaluaciones (origen TransferenciaIngreso) y se restaura al reabrir el
///   período del origen. No se puede reabrir si el período del destino ya se cerró.
/// - Es la ÚNICA excepción a que MovimientosArticulo sea de solo inserción: se actualiza el costo de la fila de
///   ingreso de la transferencia, mientras el período del destino está abierto.
/// Los asientos contables no se tocan (fuera de esta fase).
/// </summary>
public sealed class TransferenciaIngresoRevaluador : ICierreRevaluador, IPropagadorRevaluacion
{
    private static readonly OrigenMovimientoArticulo[] _origenes = { OrigenMovimientoArticulo.TransferenciaIngreso };

    private readonly IUnitOfWork _uow;

    public TransferenciaIngresoRevaluador(IUnitOfWork uow) => _uow = uow;

    public IReadOnlyCollection<OrigenMovimientoArticulo> Origenes => _origenes;

    /// <summary>
    /// El cierre no tiene "salidas" de este origen: el ingreso se corrige por propagación (PropagarAsync), no desde
    /// una lista de salidas valorizadas.
    /// </summary>
    public Task<IReadOnlyList<RevaluacionAplicada>> AplicarAsync(
        string plantaCode, string periodCode, IReadOnlyList<SalidaValorizada> salidas, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<RevaluacionAplicada>>(Array.Empty<RevaluacionAplicada>());

    public async Task<PropagacionResultado> PropagarAsync(
        string plantaCode, string periodCode, IReadOnlyList<RevaluacionAplicada> aplicadas, CancellationToken ct)
    {
        var delVale = aplicadas.Where(a => a.Origen == OrigenMovimientoArticulo.ValeSalida).ToList();
        if (delVale.Count == 0)
            return PropagacionResultado.Vacio;

        var codigos = delVale.Select(a => a.DocumentoCode).Distinct().ToList();

        // Solo los vales de transferencia.
        var transferencias = await _uow.Logistica.Transacciones.Vales.Query().AsNoTracking()
            .Where(v => v.PlantaCode == plantaCode
                        && codigos.Contains(v.Code)
                        && v.TipoValeCode == ValeRules.TipoValeTransferencia)
            .Select(v => v.Code)
            .ToListAsync(ct);

        if (transferencias.Count == 0)
            return PropagacionResultado.Vacio;

        var notas = await _uow.Logistica.Transacciones.NotaIngresos.Query()
            .Include(n => n.Detalles)
            .Where(n => n.PlantaOrigenCode == plantaCode
                        && n.ValeTransferenciaCode != null
                        && transferencias.Contains(n.ValeTransferenciaCode)
                        && n.Estado == EstadoNotaIngreso.Procesado)
            .ToListAsync(ct);

        var resultado = new List<RevaluacionAplicada>();
        var incidencias = new List<PropagacionIncidencia>();

        foreach (var nota in notas)
        {
            var lineasVale = delVale.Where(a => a.DocumentoCode == nota.ValeTransferenciaCode).ToList();

            if (await ControlCierreRules.IsPeriodClosedAsync(_uow, nota.PlantaCode, nota.FechaRecepcion, ct))
            {
                var periodoDestino = ControlCierreRules.PeriodOf(nota.FechaRecepcion);
                foreach (var l in lineasVale)
                {
                    incidencias.Add(new PropagacionIncidencia(
                        l.ArticuloCode, l.DocumentoCode, TipoIncidenciaCierre.TransferenciaDestinoCerrado,
                        $"El ingreso {nota.PlantaCode}/{nota.Code} de la transferencia {l.DocumentoCode} conserva su costo anterior: " +
                        $"el período {periodoDestino} de la planta destino ya está cerrado."));
                }
                continue;
            }

            var movimientos = await _uow.Logistica.Transacciones.MovimientosArticulo.Query()
                .Where(m => m.PlantaCode == nota.PlantaCode
                            && m.Origen == OrigenMovimientoArticulo.TransferenciaIngreso
                            && m.DocumentoCode == nota.Code
                            && !m.EsReversion)
                .ToListAsync(ct);

            var modificada = false;

            foreach (var a in lineasVale)
            {
                var detalle = nota.Detalles.FirstOrDefault(d =>
                    d.ItemNumber == a.ItemNumber && d.Estado == EstadoNotaIngresoDetalle.Procesado);

                if (detalle is null)
                    continue;

                var precioNuevo = Math.Round(a.PrecioNuevo, 6, MidpointRounding.AwayFromZero);
                if (Math.Abs(detalle.Precio - precioNuevo) < 0.0000005m)
                    continue;

                var totalNuevo = Math.Round(detalle.Cantidad * precioNuevo, 2, MidpointRounding.AwayFromZero);

                resultado.Add(new RevaluacionAplicada(
                    OrigenMovimientoArticulo.TransferenciaIngreso, nota.Code, detalle.ItemNumber, detalle.ArticuloCode,
                    detalle.Cantidad, detalle.Precio, precioNuevo, detalle.Total, totalNuevo));

                detalle.Precio = precioNuevo;
                detalle.Total = totalNuevo;

                var movimiento = movimientos.FirstOrDefault(m => m.ItemNumber == detalle.ItemNumber);
                if (movimiento is not null)
                    movimiento.CostoUnitario = precioNuevo;

                modificada = true;
            }

            if (modificada)
                await RecalcularTotalesAsync(nota, ct);
        }

        return new PropagacionResultado(resultado, incidencias);
    }

    public async Task RevertirAsync(
        string plantaCode, string periodCode, IReadOnlyList<CierreRevaluacion> auditoria, CancellationToken ct)
    {
        var propias = auditoria.Where(a => a.Origen == OrigenMovimientoArticulo.TransferenciaIngreso).ToList();
        if (propias.Count == 0)
            return;

        var codigos = propias.Select(a => a.DocumentoCode).Distinct().ToList();

        // plantaCode es la planta ORIGEN (la que se cierra); el código de la nota es único por planta destino.
        var notas = await _uow.Logistica.Transacciones.NotaIngresos.Query()
            .Include(n => n.Detalles)
            .Where(n => n.PlantaOrigenCode == plantaCode && codigos.Contains(n.Code))
            .ToListAsync(ct);

        foreach (var nota in notas)
        {
            if (await ControlCierreRules.IsPeriodClosedAsync(_uow, nota.PlantaCode, nota.FechaRecepcion, ct))
            {
                throw new InvalidOperationException(
                    $"No se puede reabrir el período {periodCode}: el ingreso {nota.PlantaCode}/{nota.Code} de una transferencia " +
                    $"pertenece al período {ControlCierreRules.PeriodOf(nota.FechaRecepcion)} de la planta destino, que ya está cerrado.");
            }

            var movimientos = await _uow.Logistica.Transacciones.MovimientosArticulo.Query()
                .Where(m => m.PlantaCode == nota.PlantaCode
                            && m.Origen == OrigenMovimientoArticulo.TransferenciaIngreso
                            && m.DocumentoCode == nota.Code
                            && !m.EsReversion)
                .ToListAsync(ct);

            foreach (var registro in propias.Where(p => p.DocumentoCode == nota.Code))
            {
                var detalle = nota.Detalles.FirstOrDefault(d => d.ItemNumber == registro.ItemNumber);
                if (detalle is null)
                    continue;

                detalle.Precio = registro.PrecioAnterior;
                detalle.Total = registro.TotalAnterior;

                var movimiento = movimientos.FirstOrDefault(m => m.ItemNumber == registro.ItemNumber);
                if (movimiento is not null)
                    movimiento.CostoUnitario = registro.PrecioAnterior;
            }

            await RecalcularTotalesAsync(nota, ct);
        }
    }

    private async Task RecalcularTotalesAsync(NotaIngreso nota, CancellationToken ct)
    {
        var comprobante = await _uow.Comunes.ComprobantesPago.Query().AsNoTracking()
            .FirstAsync(c => c.Code == nota.ComprobantePagoCode, ct);

        await NotaIngresoTotalsCalculator.ApplyOnCreateAsync(_uow, nota, comprobante, ct);
    }
}
