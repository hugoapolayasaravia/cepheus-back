using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;

/// <summary>
/// Revaloriza las líneas procesadas del vale de salida de una Orden de Servicio (MOrdenServicioDetS) y recalcula
/// neto, IGV y total del vale (Consumos / Consumos_nc del legacy).
/// El IGV sale de ControlVentas, igual que en el resto del módulo (el legacy usaba fn_Igv por fecha: ver LEEME).
/// </summary>
public sealed class OrdenServicioSalidaRevaluador : ICierreRevaluador
{
    private static readonly OrigenMovimientoArticulo[] _origenes = { OrigenMovimientoArticulo.OrdenServicio };

    private readonly IUnitOfWork _uow;

    public OrdenServicioSalidaRevaluador(IUnitOfWork uow) => _uow = uow;

    public IReadOnlyCollection<OrigenMovimientoArticulo> Origenes => _origenes;

    public async Task<IReadOnlyList<RevaluacionAplicada>> AplicarAsync(
        string plantaCode, string periodCode, IReadOnlyList<SalidaValorizada> salidas, CancellationToken ct)
    {
        var aplicadas = new List<RevaluacionAplicada>();
        if (salidas.Count == 0)
            return aplicadas;

        var codigos = salidas.Select(s => s.DocumentoCode).Distinct().ToList();

        var vales = await _uow.Logistica.Transacciones.OrdenesServicioSalida.Query()
            .Include(v => v.Detalles)
            .Where(v => v.PlantaCode == plantaCode && codigos.Contains(v.Code))
            .ToListAsync(ct);

        var modificados = new HashSet<OrdenServicioSalida>();

        foreach (var salida in salidas)
        {
            var vale = vales.FirstOrDefault(v => v.Code == salida.DocumentoCode);
            var linea = vale?.Detalles.FirstOrDefault(d =>
                d.ItemNumber == salida.ItemNumber &&
                d.Estado == EstadoOrdenServicioSalidaDetalle.Procesado);

            if (vale is null || linea is null)
                continue;

            var precioNuevo = Math.Round(salida.PrecioNuevo, 6, MidpointRounding.AwayFromZero);
            if (Math.Abs(linea.Precio - precioNuevo) < 0.0000005m)
                continue;

            var totalNuevo = Math.Round(linea.Cantidad * precioNuevo, 2, MidpointRounding.AwayFromZero);

            aplicadas.Add(new RevaluacionAplicada(
                OrigenMovimientoArticulo.OrdenServicio, vale.Code, linea.ItemNumber, linea.ArticuloCode,
                linea.Cantidad, linea.Precio, precioNuevo, linea.Total, totalNuevo));

            linea.Precio = precioNuevo;
            linea.Total = totalNuevo;
            modificados.Add(vale);
        }

        foreach (var vale in modificados)
            await OrdenServicioSalidaSync.RecalculateAsync(_uow, vale, ct);

        return aplicadas;
    }

    public async Task RevertirAsync(
        string plantaCode, string periodCode, IReadOnlyList<CierreRevaluacion> auditoria, CancellationToken ct)
    {
        var propias = auditoria.Where(a => a.Origen == OrigenMovimientoArticulo.OrdenServicio).ToList();
        if (propias.Count == 0)
            return;

        var codigos = propias.Select(a => a.DocumentoCode).Distinct().ToList();

        var vales = await _uow.Logistica.Transacciones.OrdenesServicioSalida.Query()
            .Include(v => v.Detalles)
            .Where(v => v.PlantaCode == plantaCode && codigos.Contains(v.Code))
            .ToListAsync(ct);

        foreach (var registro in propias)
        {
            var linea = vales
                .FirstOrDefault(v => v.Code == registro.DocumentoCode)?
                .Detalles.FirstOrDefault(d => d.ItemNumber == registro.ItemNumber);

            if (linea is null)
                continue;

            linea.Precio = registro.PrecioAnterior;
            linea.Total = registro.TotalAnterior;
        }

        foreach (var vale in vales)
            await OrdenServicioSalidaSync.RecalculateAsync(_uow, vale, ct);
    }
}
