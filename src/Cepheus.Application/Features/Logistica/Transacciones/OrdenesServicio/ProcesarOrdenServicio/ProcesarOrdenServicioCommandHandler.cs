using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.ProcesarOrdenServicio;

/// <summary>
/// Traducción de ue_set_btn_procesar del PowerBuilder. Todo (líneas, stock, artículo-proveedor, materiales
/// de la OT y estado) se confirma con UN solo SaveChanges, o sea una sola transacción.
///
/// Stock (Logi_sp_Actualiza_Stock_Planta_Articulo): por cada línea el legacy hacía una ENTRADA con recálculo
/// de costo promedio ponderado (precio con descuento) y enseguida una SALIDA de la misma cantidad. El efecto
/// neto es: la cantidad NO cambia y se actualizan UnitCost, UnitCostUsd y AverageCost. Se hacen las dos
/// llamadas al servicio central IArticuloStockMovementService, igual que el legacy.
///
/// El vale de salida (OrdenServicioSalida) ya existe desde que se creó la orden: aquí se marca Procesado.
/// </summary>
public sealed class ProcesarOrdenServicioCommandHandler
    : IRequestHandler<ProcesarOrdenServicioCommand, OrdenServicioResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IArticuloStockMovementService _stock;
    private readonly ICurrentUserService _currentUser;

    public ProcesarOrdenServicioCommandHandler(
        IUnitOfWork uow,
        IArticuloStockMovementService stock,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _stock = stock;
        _currentUser = currentUser;
    }

    public async Task<OrdenServicioResponse> Handle(ProcesarOrdenServicioCommand request, CancellationToken ct)
    {
        var planta = OrdenServicioRules.Normalize(request.PlantaCode);
        var code = OrdenServicioRules.Normalize(request.Code);

        var os = await _uow.Logistica.Transacciones.OrdenesServicio.Query()
            .Include(x => x.Detalles)
            .Include(x => x.ValeSalida).ThenInclude(v => v.Detalles)
            .FirstOrDefaultAsync(x => x.PlantaCode == planta && x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Orden de Servicio {planta}/{code} no existe.");

        // PB: solo se procesa una orden aprobada.
        if (os.Estado != EstadoOrdenServicio.Aprobado)
            throw new InvalidOperationException(
                $"La Orden de Servicio está en estado '{os.Estado}'. Solo se puede procesar una orden Aprobada.");

        await NotaIngresoRules.EnsurePeriodOpenAsync(
            _uow, planta, os.FechaRecepcion,
            "La Fecha de Recepción no puede ser menor o igual al cierre del período.", ct);

        if (os.ComprobantePagoCode != OrdenServicioRules.ComprobanteGuia &&
            string.IsNullOrWhiteSpace(os.NumeroDocumento))
        {
            throw new InvalidOperationException("Debe indicar el número de documento antes de procesar.");
        }

        var lineas = os.Detalles
            .Where(d => d.Estado == EstadoOrdenServicioDetalle.Pendiente)
            .OrderBy(d => d.ItemNumber)
            .ToList();

        if (lineas.Count == 0)
            throw new InvalidOperationException("La Orden de Servicio no tiene líneas pendientes por procesar.");

        // PB: el tipo de vale Transferencia no lo procesa el almacén origen.
        if (lineas.Any(l => l.TipoValeCode == OrdenServicioRules.TipoValeTransferencia))
            throw new InvalidOperationException(
                "El tipo de vale Transferencia (TRA) no puede ser procesado por el almacén origen.");

        var articulosProveedor = new HashSet<string>();

        foreach (var linea in lineas)
        {
            var (precioSoles, precioDolares) = NotaIngresoRules.ConvertPrices(os.MonedaCode, linea.Precio, os.TipoCambio);

            // 1) Entrada con recálculo de costo promedio (Logi_sp_Actualiza_Stock_Planta_Articulo, @wOpe='+', @Calculo='S').
            var contextoOs = new StockMovementContext(
                OrigenMovimientoArticulo.OrdenServicio, os.Code, linea.ItemNumber, os.FechaRecepcion);

            await _stock.ApplyAsync(
                planta, linea.ArticuloCode, linea.Cantidad,
                precioSoles, precioDolares, linea.Descuento,
                increment: true, recalculateAverage: true,
                context: contextoOs, cancellationToken: ct);

            // 2) Salida de la misma cantidad (@wOpe='-', @Calculo='N'): deja la cantidad como estaba.
            await _stock.ApplyAsync(
                planta, linea.ArticuloCode, linea.Cantidad, 0m, 0m, 0m,
                increment: false, recalculateAverage: false,
                context: contextoOs, cancellationToken: ct);

            // Logi_sp_AgregaMArticulo_Proveedor: relaciona el artículo con el proveedor si aún no existe.
            if (articulosProveedor.Add(linea.ArticuloCode))
            {
                var existe = await _uow.Logistica.Maestros.ArticuloProveedores.Query()
                    .AnyAsync(x =>
                        x.PlantaCode == planta &&
                        x.ArticuloCode == linea.ArticuloCode &&
                        x.ProveedorCode == os.ProveedorCode, ct);

                if (!existe)
                {
                    await _uow.Logistica.Maestros.ArticuloProveedores.AddAsync(new ArticuloProveedor
                    {
                        PlantaCode = planta,
                        ArticuloCode = linea.ArticuloCode,
                        ProveedorCode = os.ProveedorCode,
                        IsAgreement = false
                    }, ct);
                }
            }

            linea.Estado = EstadoOrdenServicioDetalle.Procesado;
        }

        // Logi_sp_AgregaTOTRMateriales: materiales de la Orden de Trabajo (solo líneas con OT).
        await OrdenServicioOtMaterialSync.ApplyAsync(_uow, os, lineas, ct);

        os.Estado = EstadoOrdenServicio.Procesado;
        os.ProcesadoPor = _currentUser.FullName;

        // Vale de salida: procesado con el mismo usuario y fecha (Login_prs / Fecha_prs de ResS).
        OrdenServicioSalidaSync.MarkProcessed(os, os.ProcesadoPor, DateTime.Now);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "La Orden de Servicio o el stock fueron modificados por otro usuario. Recargue e intente nuevamente.");
        }
        catch (DbUpdateException ex)
        {
            var detalleError = ex.InnerException?.Message ?? ex.Message;
            throw new InvalidOperationException($"Error al procesar la Orden de Servicio: {detalleError}", ex);
        }

        return await OrdenServicioReader.GetAsync(_uow, planta, code, ct);
    }
}
