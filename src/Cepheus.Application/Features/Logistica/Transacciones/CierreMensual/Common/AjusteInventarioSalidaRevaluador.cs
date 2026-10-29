using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;

/// <summary>
/// Revaloriza las líneas procesadas de FALTANTE del Ajuste de Inventario (salidas, 'AS' del legacy) y recalcula neto,
/// IGV y total del ajuste. Los sobrantes (ingresos, 'AI') conservan el precio del ajuste: es su costo de ingreso.
/// </summary>
public sealed class AjusteInventarioSalidaRevaluador : ICierreRevaluador
{
    private static readonly OrigenMovimientoArticulo[] _origenes = { OrigenMovimientoArticulo.AjusteSalida };

    private readonly IUnitOfWork _uow;

    public AjusteInventarioSalidaRevaluador(IUnitOfWork uow) => _uow = uow;

    public IReadOnlyCollection<OrigenMovimientoArticulo> Origenes => _origenes;

    public async Task<IReadOnlyList<RevaluacionAplicada>> AplicarAsync(
        string plantaCode, string periodCode, IReadOnlyList<SalidaValorizada> salidas, CancellationToken ct)
    {
        var aplicadas = new List<RevaluacionAplicada>();
        if (salidas.Count == 0)
            return aplicadas;

        var codigos = salidas.Select(s => s.DocumentoCode).Distinct().ToList();

        var ajustes = await _uow.Logistica.Transacciones.AjustesInventario.Query()
            .Include(a => a.Detalles)
            .Where(a => a.PlantaCode == plantaCode && codigos.Contains(a.Code))
            .ToListAsync(ct);

        var modificados = new HashSet<AjusteInventario>();

        foreach (var salida in salidas)
        {
            var ajuste = ajustes.FirstOrDefault(a => a.Code == salida.DocumentoCode);
            var linea = ajuste?.Detalles.FirstOrDefault(d =>
                d.ItemNumber == salida.ItemNumber &&
                d.Tipo == TipoAjusteInventario.Faltante &&
                d.Estado == EstadoAjusteInventarioDetalle.Procesado);

            if (ajuste is null || linea is null)
                continue;

            var precioNuevo = Math.Round(salida.PrecioNuevo, 6, MidpointRounding.AwayFromZero);
            if (Math.Abs(linea.Precio - precioNuevo) < 0.0000005m)
                continue;

            var totalNuevo = Math.Round(linea.Cantidad * precioNuevo, 2, MidpointRounding.AwayFromZero);

            aplicadas.Add(new RevaluacionAplicada(
                OrigenMovimientoArticulo.AjusteSalida, ajuste.Code, linea.ItemNumber, linea.ArticuloCode,
                linea.Cantidad, linea.Precio, precioNuevo, linea.Total, totalNuevo));

            linea.Precio = precioNuevo;
            linea.Total = totalNuevo;
            modificados.Add(ajuste);
        }

        foreach (var ajuste in modificados)
            await AjusteInventarioRules.RecalculateTotalsAsync(_uow, ajuste, ct);

        return aplicadas;
    }

    public async Task RevertirAsync(
        string plantaCode, string periodCode, IReadOnlyList<CierreRevaluacion> auditoria, CancellationToken ct)
    {
        var propias = auditoria.Where(a => a.Origen == OrigenMovimientoArticulo.AjusteSalida).ToList();
        if (propias.Count == 0)
            return;

        var codigos = propias.Select(a => a.DocumentoCode).Distinct().ToList();

        var ajustes = await _uow.Logistica.Transacciones.AjustesInventario.Query()
            .Include(a => a.Detalles)
            .Where(a => a.PlantaCode == plantaCode && codigos.Contains(a.Code))
            .ToListAsync(ct);

        foreach (var registro in propias)
        {
            var linea = ajustes
                .FirstOrDefault(a => a.Code == registro.DocumentoCode)?
                .Detalles.FirstOrDefault(d => d.ItemNumber == registro.ItemNumber);

            if (linea is null)
                continue;

            linea.Precio = registro.PrecioAnterior;
            linea.Total = registro.TotalAnterior;
        }

        foreach (var ajuste in ajustes)
            await AjusteInventarioRules.RecalculateTotalsAsync(_uow, ajuste, ct);
    }
}
