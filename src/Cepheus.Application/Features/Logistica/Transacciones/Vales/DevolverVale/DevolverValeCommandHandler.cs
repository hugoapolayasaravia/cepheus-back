using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.DevolverVale;

public sealed class DevolverValeCommandHandler
    : IRequestHandler<DevolverValeCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IArticuloStockMovementService _stock;

    public DevolverValeCommandHandler(
        IUnitOfWork uow,
        IArticuloStockMovementService stock)
    {
        _uow = uow;
        _stock = stock;
    }

    public async Task<ValeResponse> Handle(
        DevolverValeCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var code = ValeRules.Normalize(request.Code);

        var vale = await ValeReader.LoadForUpdateAsync(_uow, planta, code, ct);

        if (vale.Estado is not (EstadoVale.Procesado or EstadoVale.EntregaParcial))
        {
            throw new InvalidOperationException(
                $"El Vale de Salida se encuentra {vale.Estado}; solo se devuelve desde Procesado o Entrega Parcial.");
        }

        if (!string.IsNullOrWhiteSpace(vale.AsientoContable))
        {
            throw new InvalidOperationException(
                "El Vale de Salida ya generó asiento contable. No se puede hacer una devolución.");
        }

        if (ValeRules.EsTransferencia(vale))
        {
            throw new InvalidOperationException(
                "Un vale de tipo Transferencia no puede ser devuelto por el almacén origen.");
        }

        await ValeRules.EnsurePeriodOpenAsync(
            _uow,
            planta,
            vale.FechaEntrega,
            "El Vale de Salida está procesado y su período está cerrado. Verifique el cierre.",
            ct);

        // Solo lo procesado se puede devolver (no anuladas, pendientes ni ya devueltas).
        var procesadas = vale.Detalles
            .Where(d => d.Estado == EstadoValeDetalle.Procesado)
            .ToList();

        if (procesadas.Count == 0)
        {
            throw new InvalidOperationException(
                "El Vale de Salida no tiene líneas procesadas para devolver.");
        }

        var seleccion = procesadas;

        if (request.Articulos is { Count: > 0 })
        {
            var solicitados = request.Articulos
                .Select(ValeRules.Normalize)
                .Distinct()
                .ToList();

            var noProcesadas = solicitados
                .Where(a => procesadas.All(p => p.ArticuloCode != a))
                .ToList();

            if (noProcesadas.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Los artículos {string.Join(", ", noProcesadas)} no son líneas procesadas del vale.");
            }

            seleccion = procesadas
                .Where(p => solicitados.Contains(p.ArticuloCode))
                .ToList();
        }

        foreach (var linea in seleccion.OrderBy(l => l.ItemNumber))
        {
            // Devolución: suma stock a costo 0 sin recalcular el promedio (Calculo 'N').
            // Kardex: deshace la salida original de la línea (mismo vale e ítem) al costo con que salió.
            await _stock.ApplyAsync(
                planta,
                linea.ArticuloCode,
                linea.Cantidad,
                0m,
                0m,
                0m,
                increment: true,
                recalculateAverage: false,
                context: new StockMovementContext(
                    OrigenMovimientoArticulo.ValeSalida, vale.Code, linea.ItemNumber, vale.FechaEntrega,
                    EsReversion: true),
                cancellationToken: ct);

            linea.Estado = EstadoValeDetalle.Devuelto;

            await ValeOrdenTrabajo.ReturnAsync(_uow, vale, linea, ct);
        }

        var quedanProcesadas = vale.Detalles
            .Any(d => d.Estado == EstadoValeDetalle.Procesado);
        var quedanPendientes = vale.Detalles
            .Any(d => d.Estado == EstadoValeDetalle.Pendiente);

        if (!quedanProcesadas && !quedanPendientes)
        {
            vale.Estado = EstadoVale.Devuelto;
        }
        else
        {
            // Devolución parcial: el vale queda con entregas parciales y los importes excluyen lo devuelto.
            vale.Estado = EstadoVale.EntregaParcial;
            await ValeRules.RecalculateTotalsAsync(_uow, vale, ct);
        }

        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, code, ct);
    }
}
