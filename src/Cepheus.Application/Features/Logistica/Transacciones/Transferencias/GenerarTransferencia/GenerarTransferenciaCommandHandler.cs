using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Transferencias.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Transferencias.GenerarTransferencia;

/// <summary>
/// ue_transferir del Maestro de Nota de Ingresos (legacy). El vale TRA se envía físicamente y NO mueve stock en el
/// origen; recién cuando llega, el destino genera la Nota de Ingreso y es ahí donde se resta del origen y se suma al
/// destino:
///   - Nota de Ingreso: comprobante 00 (ninguno), número 'TRANSFERENCIA'+origen, proveedor de transferencias, soles,
///     forma de pago 10, origen Transferencia, IGV cero, todas las líneas del vale a su precio, sin descuento, procesadas.
///   - Destino: ingreso que recalcula el costo promedio con el precio del vale (Calculo 'S').
///   - Origen: salida sin recalcular (Calculo 'N'); la verificación de stock suficiente la hace el servicio de stock.
///   - Vale: líneas y cabecera Procesado.
/// Una sola transacción (un SaveChanges): si algo falla no queda nada a medias. Los asientos contables no se generan
/// (fuera de esta fase). La anulación de la transferencia se hace desde la anulación de la Nota de Ingreso
/// (AnulateNotaIngresoCommandHandler). Dos recepciones simultáneas del mismo vale: la segunda falla por el RowVersion
/// del vale.
/// Ambos movimientos se fechan con la fecha de recepción (cuando el stock realmente se mueve) y respetan el período
/// cerrado de cada planta; el legacy dejaba la salida del origen con la fecha del vale.
/// </summary>
public sealed class GenerarTransferenciaCommandHandler : IRequestHandler<GenerarTransferenciaCommand, NotaIngresoResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IArticuloStockMovementService _stock;
    private readonly ITransferenciaSettings _settings;
    private readonly ICurrentUserService _currentUser;

    public GenerarTransferenciaCommandHandler(
        IUnitOfWork uow,
        IArticuloStockMovementService stock,
        ITransferenciaSettings settings,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _stock = stock;
        _settings = settings;
        _currentUser = currentUser;
    }

    public async Task<NotaIngresoResponse> Handle(GenerarTransferenciaCommand request, CancellationToken ct)
    {
        var destino = NotaIngresoRules.Normalize(request.PlantaDestinoCode);
        var origen = NotaIngresoRules.Normalize(request.PlantaOrigenCode);
        var valeCode = NotaIngresoRules.Normalize(request.ValeCode);
        var fechaRecepcion = request.FechaRecepcion;
        var fechaEmision = request.FechaEmision ?? request.FechaRecepcion;

        if (destino == origen)
            throw new InvalidOperationException("La planta origen no puede ser la misma que la planta destino.");

        var plantas = await _uow.Comunes.Plantas.Query().AsNoTracking()
            .Where(p => p.Code == destino || p.Code == origen)
            .Select(p => p.Code)
            .ToListAsync(ct);

        if (!plantas.Contains(destino))
            throw new KeyNotFoundException($"La planta {destino} no existe.");
        if (!plantas.Contains(origen))
            throw new KeyNotFoundException($"La planta {origen} no existe.");

        // ------------------------------------------------------------ el vale
        var vale = await _uow.Logistica.Transacciones.Vales.Query()
            .Include(v => v.Detalles)
            .FirstOrDefaultAsync(v => v.PlantaCode == origen && v.Code == valeCode, ct)
            ?? throw new KeyNotFoundException($"El vale {origen}/{valeCode} no existe.");

        if (!ValeRules.EsTransferencia(vale))
            throw new InvalidOperationException($"El vale {origen}/{valeCode} no es de tipo Transferencia.");

        if (vale.PlantaAfectadaCode != destino)
            throw new InvalidOperationException(
                $"El vale {origen}/{valeCode} está dirigido a la planta {vale.PlantaAfectadaCode}, no a {destino}.");

        if (vale.Estado != EstadoVale.Pendiente)
            throw new InvalidOperationException(
                $"El vale {origen}/{valeCode} se encuentra {vale.Estado}: solo se recibe un vale pendiente.");

        var lineas = vale.Detalles
            .Where(d => d.Estado == EstadoValeDetalle.Pendiente)
            .OrderBy(d => d.ItemNumber)
            .ToList();

        if (lineas.Count == 0)
            throw new InvalidOperationException($"El vale {origen}/{valeCode} no tiene líneas pendientes.");

        var yaRecibido = await _uow.Logistica.Transacciones.NotaIngresos.Query().AsNoTracking()
            .AnyAsync(n => n.PlantaOrigenCode == origen
                           && n.ValeTransferenciaCode == valeCode
                           && n.Estado != EstadoNotaIngreso.Anulado, ct);

        if (yaRecibido)
            throw new InvalidOperationException($"El vale {origen}/{valeCode} ya fue recibido en una transferencia.");

        // ------------------------------------------------------------ fechas (4 meses, no futuras, período abierto)
        await NotaIngresoRules.ValidateDatesAsync(_uow, destino, fechaEmision, fechaRecepcion, ct);

        // ------------------------------------------------------------ catálogos que usa la nota
        var comprobante = await _uow.Comunes.ComprobantesPago.Query().AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == TransferenciaRules.ComprobanteNinguno, ct);
        if (comprobante is null || !comprobante.IsActive)
            throw new InvalidOperationException(
                $"No existe el comprobante de pago '{TransferenciaRules.ComprobanteNinguno}' (NINGUNO) que usan las transferencias.");

        var proveedorCode = _settings.ProveedorCode;
        if (!await _uow.Logistica.Maestros.Proveedores.Query().AsNoTracking().AnyAsync(p => p.Code == proveedorCode, ct))
            throw new InvalidOperationException(
                $"No existe el proveedor '{proveedorCode}' de las transferencias. Configure Logistica:ProveedorTransferenciasCode con un proveedor existente.");

        if (!await _uow.Logistica.Catalogos.FormasPago.Query().AsNoTracking().AnyAsync(f => f.Code == TransferenciaRules.FormaPagoSegunNotas, ct))
            throw new InvalidOperationException(
                $"No existe la forma de pago '{TransferenciaRules.FormaPagoSegunNotas}' (SEGUN NOTAS) que usan las transferencias.");

        if (!await _uow.Comunes.Monedas.Query().AsNoTracking().AnyAsync(m => m.Code == ValeRules.MonedaSoles, ct))
            throw new InvalidOperationException($"No existe la moneda '{ValeRules.MonedaSoles}'.");

        // ------------------------------------------------------------ la Nota de Ingreso
        var note = new NotaIngreso
        {
            PlantaCode = destino,
            Condicion = CondicionNotaIngreso.OrdenCompra,
            Origen = OrigenNotaIngreso.Transferencia,
            ComprobantePagoCode = comprobante.Code,
            NumeroDocumento = TransferenciaRules.NumeroDocumento(origen),
            ProveedorCode = proveedorCode,
            MonedaCode = ValeRules.MonedaSoles,
            FormaPagoCode = TransferenciaRules.FormaPagoSegunNotas,
            TipoCambio = 0m,
            FechaEmision = fechaEmision,
            FechaRecepcion = fechaRecepcion,
            FechaProceso = DateTime.Now,
            Estado = EstadoNotaIngreso.Procesado,
            PlantaOrigenCode = origen,
            ValeTransferenciaCode = valeCode
        };

        note.Code = await NotaIngresoRules.NextCodeAsync(_uow, destino, ct);

        foreach (var linea in lineas)
        {
            note.Detalles.Add(new NotaIngresoDetalle
            {
                PlantaCode = destino,
                NotaIngresoCode = note.Code,
                // Mismo ítem que la línea del vale: así el cierre del origen encuentra la línea a corregir.
                ItemNumber = linea.ItemNumber,
                ArticuloCode = linea.ArticuloCode,
                Cantidad = linea.Cantidad,
                Precio = linea.Precio,
                Descuento = 0m,
                Total = Math.Round(linea.Cantidad * linea.Precio, 2, MidpointRounding.AwayFromZero),
                Estado = EstadoNotaIngresoDetalle.Procesado
            });
        }

        await NotaIngresoTotalsCalculator.ApplyOnCreateAsync(_uow, note, comprobante, ct);

        // ------------------------------------------------------------ stock y Kardex de ambas plantas
        foreach (var linea in lineas)
        {
            // Origen primero: si no alcanza el stock, falla antes de tocar nada más.
            await _stock.ApplyAsync(
                origen, linea.ArticuloCode, linea.Cantidad,
                0m, 0m, 0m,
                increment: false, recalculateAverage: false,
                context: new StockMovementContext(
                    OrigenMovimientoArticulo.ValeSalida, vale.Code, linea.ItemNumber, fechaRecepcion),
                cancellationToken: ct);

            // Destino: ingreso al precio del vale, recalculando el costo promedio.
            await _stock.ApplyAsync(
                destino, linea.ArticuloCode, linea.Cantidad,
                linea.Precio, 0m, 0m,
                increment: true, recalculateAverage: true,
                context: new StockMovementContext(
                    OrigenMovimientoArticulo.TransferenciaIngreso, note.Code, linea.ItemNumber, fechaRecepcion),
                cancellationToken: ct);

            linea.Estado = EstadoValeDetalle.Procesado;
        }

        vale.Estado = EstadoVale.Procesado;
        vale.ProcesadoPor = _currentUser.FullName;
        vale.FechaProcesado = DateTime.Now;
        vale.Preparado = false;

        await _uow.Logistica.Transacciones.NotaIngresos.AddAsync(note, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "El stock o el vale cambió mientras se procesaba la transferencia. Intente nuevamente.");
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(
                $"Error al guardar la transferencia: {ex.InnerException?.Message ?? ex.Message}", ex);
        }

        return await NotaIngresoReader.GetAsync(_uow, destino, note.Code, ct);
    }
}
