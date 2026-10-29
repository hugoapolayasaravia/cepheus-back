using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.CreateNotaIngreso;

/// <summary>
/// Todo el registro (nota, líneas, entregado en la OC, stock, artículo-proveedor) se confirma con UN
/// solo SaveChanges, o sea una sola transacción: si algo falla no queda nada a medias.
/// </summary>
public sealed class CreateNotaIngresoCommandHandler
    : IRequestHandler<CreateNotaIngresoCommand, NotaIngresoResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IArticuloStockMovementService _stock;

    public CreateNotaIngresoCommandHandler(
        IUnitOfWork uow,
        IArticuloStockMovementService stock)
    {
        _uow = uow;
        _stock = stock;
    }

    public async Task<NotaIngresoResponse> Handle(
        CreateNotaIngresoCommand request,
        CancellationToken ct)
    {
        var planta = NotaIngresoRules.Normalize(request.PlantaCode);

        var comprobante = await _uow.Comunes.ComprobantesPago
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.Code == request.ComprobantePagoCode,
                ct);

        if (comprobante is null ||
            !comprobante.IsActive ||
            !comprobante.AvailableForPurchaseOrder)
        {
            throw new InvalidOperationException(
                "El comprobante de pago no existe o no admite Nota de Ingreso.");
        }

        var esNotaCredito =
            comprobante.Code == NotaIngresoRules.ComprobanteNotaCredito;

        var esGuia =
            comprobante.Code == NotaIngresoRules.ComprobanteGuia;

        // PB: la guía (09) no lleva número de documento; los demás sí.
        var numeroDocumento =
            esGuia
                ? null
                : request.NumeroDocumento?.Trim();

        var numeroGuia =
            request.NumeroGuia?.Trim();

        if (!esGuia &&
            string.IsNullOrWhiteSpace(numeroDocumento))
        {
            throw new InvalidOperationException(
                "Debe indicar el número de documento.");
        }

        if (esGuia &&
            string.IsNullOrWhiteSpace(numeroGuia))
        {
            throw new InvalidOperationException(
                "Debe indicar el número de guía.");
        }

        var repetidas = request.Detalles
            .GroupBy(d => (
                NotaIngresoRules.Normalize(d.ArticuloCode),
                NotaIngresoRules.NormalizeOrNull(d.PedidoCode)))
            .Any(g => g.Count() > 1);

        if (repetidas)
        {
            throw new InvalidOperationException(
                "Un mismo artículo (y pedido) no puede repetirse en la Nota de Ingreso.");
        }

        await NotaIngresoRules.ValidateDatesAsync(
            _uow,
            planta,
            request.FechaEmision,
            request.FechaRecepcion,
            ct);

        // Movimientos de Kardex generados por las líneas; el documento aún no tiene correlativo.
        var movimientos = new List<MovimientoArticulo>();

        var note = esNotaCredito
            ? await BuildNotaCreditoAsync(
                request,
                planta,
                movimientos,
                ct)
            : await BuildDesdeOrdenCompraAsync(
                request,
                planta,
                movimientos,
                ct);

        note.PlantaCode = planta;
        note.Condicion = request.Condicion;
        note.Origen = request.Origen;
        note.ComprobantePagoCode = comprobante.Code;
        note.NumeroDocumento = numeroDocumento;
        note.NumeroGuia = esNotaCredito
            ? null
            : numeroGuia;
        note.FechaEmision = request.FechaEmision;
        note.FechaRecepcion = request.FechaRecepcion;
        note.FechaProceso = DateTime.Now;
        note.Estado = EstadoNotaIngreso.Procesado;

        // PB: no se repite (tipo, proveedor, número)
        // entre notas no anuladas, salvo guías.
        if (!esGuia)
        {
            var duplicado =
                await _uow.Logistica.Transacciones.NotaIngresos
                    .Query()
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.ComprobantePagoCode ==
                                comprobante.Code &&
                            x.ProveedorCode ==
                                note.ProveedorCode &&
                            x.NumeroDocumento ==
                                numeroDocumento &&
                            x.Estado !=
                                EstadoNotaIngreso.Anulado,
                        ct);

            if (duplicado)
            {
                throw new InvalidOperationException(
                    "Ya existe una Nota de Ingreso con el mismo tipo de documento, proveedor y número.");
            }
        }

        await NotaIngresoTotalsCalculator.ApplyOnCreateAsync(
            _uow,
            note,
            comprobante,
            ct);

        note.Code =
            await NotaIngresoRules.NextCodeAsync(
                _uow,
                planta,
                ct);

        foreach (var movimiento in movimientos)
        {
            movimiento.DocumentoCode = note.Code;
        }

        foreach (var detalle in note.Detalles)
        {
            detalle.NotaIngresoCode = note.Code;
        }

        await _uow.Logistica.Transacciones.NotaIngresos
            .AddAsync(
                note,
                ct);

        // ============================================================
        // GUARDAR
        // ============================================================

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            var detalleError =
                ex.InnerException?.Message ??
                ex.Message;

            throw new InvalidOperationException(
                $"Error al guardar la Nota de Ingreso: {detalleError}",
                ex);
        }

        return await NotaIngresoReader.GetAsync(
            _uow,
            planta,
            note.Code,
            ct);
    }

    // ================================================================
    // RECEPCIÓN CONTRA ORDEN DE COMPRA
    // ================================================================

    private async Task<NotaIngreso> BuildDesdeOrdenCompraAsync(
        CreateNotaIngresoCommand request,
        string planta,
        List<MovimientoArticulo> movimientos,
        CancellationToken ct)
    {
        var ocCode =
            NotaIngresoRules.NormalizeOrNull(
                request.OrdenCompraCode)
            ?? throw new InvalidOperationException(
                "Debe indicar la Orden de Compra.");

        var oc =
            await _uow.Logistica.Transacciones.OrdenesCompra
                .Query()
                .Include(x => x.Detalles)
                    .ThenInclude(d => d.Origenes)
                .FirstOrDefaultAsync(
                    x =>
                        x.PlantaCode == planta &&
                        x.Code == ocCode,
                    ct)
            ?? throw new KeyNotFoundException(
                $"La Orden de Compra {ocCode} no existe en la planta {planta}.");

        NotaIngresoOrdenCompraUpdater.EnsureReceivable(oc);

        var tipoCambio =
            await NotaIngresoRules.ResolveTipoCambioAsync(
                _uow,
                oc.MonedaCode,
                request.FechaEmision,
                ct);

        var note = new NotaIngreso
        {
            OrdenCompraCode = oc.Code,
            ProveedorCode = oc.ProveedorCode,
            MonedaCode = oc.MonedaCode,
            FormaPagoCode = oc.FormaPagoCode,
            TipoCambio = tipoCambio,
            FechaDocumento = request.FechaEmision
        };

        var item = 0;

        var articulosProveedor =
            new HashSet<string>();

        foreach (var linea in request.Detalles)
        {
            var articulo =
                NotaIngresoRules.Normalize(
                    linea.ArticuloCode);

            var pedido =
                NotaIngresoRules.NormalizeOrNull(
                    linea.PedidoCode);

            var ocDetalle =
                oc.Detalles.FirstOrDefault(
                    d => d.ArticuloCode == articulo)
                ?? throw new InvalidOperationException(
                    $"El artículo {articulo} no pertenece a la Orden de Compra {oc.Code}.");

            if (ocDetalle.Origenes.Count > 0 &&
                pedido is null)
            {
                throw new InvalidOperationException(
                    $"El artículo {articulo} proviene de pedidos: indique el pedido de la línea.");
            }

            if (ocDetalle.Origenes.Count == 0 &&
                pedido is not null)
            {
                throw new InvalidOperationException(
                    $"El artículo {articulo} es una línea directa de la OC y no lleva pedido.");
            }

            if (linea.Precio <= 0)
            {
                throw new InvalidOperationException(
                    $"El precio del artículo {articulo} debe ser mayor a cero.");
            }

            if (linea.Precio >
                ocDetalle.PrecioArticulo)
            {
                throw new InvalidOperationException(
                    $"El precio del artículo {articulo} ({linea.Precio}) no puede ser mayor al de la Orden de Compra ({ocDetalle.PrecioArticulo}).");
            }

            // Actualiza la cantidad entregada de la OC.
            NotaIngresoOrdenCompraUpdater.ApplyDelivery(
                ocDetalle,
                pedido,
                linea.Cantidad);

            var total =
                NotaIngresoRules.LineTotal(
                    linea.Cantidad,
                    linea.Precio,
                    linea.Descuento);

            if (total <= 0)
            {
                throw new InvalidOperationException(
                    $"El total del artículo {articulo} debe ser mayor a cero.");
            }

            note.Detalles.Add(
                new NotaIngresoDetalle
                {
                    PlantaCode = planta,
                    ArticuloCode = articulo,
                    PedidoCode = pedido,
                    ItemNumber = ++item,
                    Cantidad = linea.Cantidad,
                    Precio = linea.Precio,
                    Descuento = linea.Descuento,
                    Total = total,
                    Estado =
                        EstadoNotaIngresoDetalle.Procesado
                });

            // ========================================================
            // STOCK
            // ========================================================

            var (
                precioSoles,
                precioDolares) =
                NotaIngresoRules.ConvertPrices(
                    oc.MonedaCode,
                    linea.Precio,
                    tipoCambio);

            var resultado = await _stock.ApplyAsync(
                planta,
                articulo,
                linea.Cantidad,
                precioSoles,
                precioDolares,
                linea.Descuento,
                increment: true,
                recalculateAverage: true,
                context: new StockMovementContext(
                    OrigenMovimientoArticulo.NotaIngreso,
                    DocumentoCode: null, // se asigna al generar el correlativo de la nota
                    ItemNumber: item,
                    FechaMovimiento: request.FechaRecepcion),
                cancellationToken: ct);

            movimientos.Add(resultado.Movimiento);

            // ========================================================
            // ARTÍCULO - PROVEEDOR
            // ========================================================

            if (articulosProveedor.Add(articulo))
            {
                var existe =
                    await _uow.Logistica.Maestros.ArticuloProveedores
                        .Query()
                        .AnyAsync(
                            x =>
                                x.PlantaCode == planta &&
                                x.ArticuloCode == articulo &&
                                x.ProveedorCode ==
                                    oc.ProveedorCode,
                            ct);

                if (!existe)
                {
                    await _uow.Logistica.Maestros
                        .ArticuloProveedores
                        .AddAsync(
                            new ArticuloProveedor
                            {
                                PlantaCode = planta,
                                ArticuloCode = articulo,
                                ProveedorCode =
                                    oc.ProveedorCode,
                                IsAgreement = false
                            },
                            ct);
                }
            }
        }

        NotaIngresoOrdenCompraUpdater.RecalculateEstado(oc);

        return note;
    }

    // ================================================================
    // NOTA DE CRÉDITO
    // ================================================================

    private async Task<NotaIngreso> BuildNotaCreditoAsync(
        CreateNotaIngresoCommand request,
        string planta,
        List<MovimientoArticulo> movimientos,
        CancellationToken ct)
    {
        var proveedorCode =
            NotaIngresoRules.NormalizeOrNull(
                request.ProveedorCode)
            ?? throw new InvalidOperationException(
                "Debe indicar el proveedor de la Nota de Crédito.");

        var referenciaCode =
            request.ComprobantePagoReferenciaCode
            ?? throw new InvalidOperationException(
                "Debe indicar el tipo de documento de referencia.");

        var numeroReferencia =
            request.NumeroReferencia?.Trim();

        if (string.IsNullOrWhiteSpace(numeroReferencia))
        {
            throw new InvalidOperationException(
                "Debe indicar el número del documento de referencia.");
        }

        var motivoCode =
            request.MotivoDevolucionCode
            ?? throw new InvalidOperationException(
                "Debe indicar el motivo de la Nota de Crédito.");

        var motivo =
            await _uow.Logistica.Catalogos.MotivosDevolucionArticulo
                .Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    m => m.Code == motivoCode,
                    ct);

        if (motivo is null ||
            !motivo.IsActive ||
            (motivo.Code != NotaIngresoRules.MotivoAjuste &&
             motivo.Code != NotaIngresoRules.MotivoDevolucion))
        {
            throw new InvalidOperationException(
                "El motivo no existe o no es válido para una Nota de Crédito (solo 01 ajuste o 02 devolución).");
        }

        var original =
            await _uow.Logistica.Transacciones.NotaIngresos
                .Query()
                .AsNoTracking()
                .Include(x => x.Detalles)
                .Where(
                    x =>
                        x.PlantaCode == planta &&
                        x.ComprobantePagoCode == referenciaCode &&
                        x.ProveedorCode == proveedorCode &&
                        x.NumeroDocumento == numeroReferencia &&
                        x.Estado != EstadoNotaIngreso.Anulado)
                .OrderByDescending(x => x.FechaProceso)
                .FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException(
                "No se encontró una Nota de Ingreso vigente para el documento referenciado.");

        var descuentaStock =
            motivo.Code ==
            NotaIngresoRules.MotivoDevolucion;

        var note = new NotaIngreso
        {
            OrdenCompraCode = null,
            ProveedorCode = original.ProveedorCode,
            MonedaCode = original.MonedaCode,
            FormaPagoCode = original.FormaPagoCode,
            TipoCambio = original.TipoCambio,
            FechaDocumento = original.FechaEmision,
            MotivoDevolucionCode = motivo.Code,
            ComprobantePagoReferenciaCode =
                referenciaCode,
            NumeroReferencia = numeroReferencia
        };

        var item = 0;

        foreach (var linea in request.Detalles)
        {
            var articulo =
                NotaIngresoRules.Normalize(
                    linea.ArticuloCode);

            var pedido =
                NotaIngresoRules.NormalizeOrNull(
                    linea.PedidoCode);

            var origDetalle =
                original.Detalles.FirstOrDefault(
                    d =>
                        d.ArticuloCode == articulo &&
                        d.PedidoCode == pedido &&
                        d.Estado ==
                            EstadoNotaIngresoDetalle.Procesado)
                ?? throw new InvalidOperationException(
                    $"El artículo {articulo} no pertenece a la Nota de Ingreso referenciada.");

            decimal cantidad;
            decimal precio;
            decimal descuento;

            if (descuentaStock)
            {
                if (linea.Cantidad >
                    origDetalle.Cantidad)
                {
                    throw new InvalidOperationException(
                        $"La cantidad del artículo {articulo} no puede superar la de la nota referenciada ({origDetalle.Cantidad}).");
                }

                cantidad = linea.Cantidad;
                precio = origDetalle.Precio;
                descuento = origDetalle.Descuento;
            }
            else
            {
                if (linea.Cantidad !=
                    origDetalle.Cantidad)
                {
                    throw new InvalidOperationException(
                        $"En un ajuste (motivo 01) la cantidad del artículo {articulo} debe ser {origDetalle.Cantidad}.");
                }

                if (linea.Precio <= 0)
                {
                    throw new InvalidOperationException(
                        $"El precio del artículo {articulo} debe ser mayor a cero.");
                }

                cantidad = origDetalle.Cantidad;
                precio = linea.Precio;
                descuento = linea.Descuento;
            }

            var total =
                NotaIngresoRules.LineTotal(
                    cantidad,
                    precio,
                    descuento);

            if (total <= 0)
            {
                throw new InvalidOperationException(
                    $"El total del artículo {articulo} debe ser mayor a cero.");
            }

            note.Detalles.Add(
                new NotaIngresoDetalle
                {
                    PlantaCode = planta,
                    ArticuloCode = articulo,
                    PedidoCode = pedido,
                    ItemNumber = ++item,
                    Cantidad = cantidad,
                    Precio = precio,
                    Descuento = descuento,
                    Total = total,
                    Estado =
                        EstadoNotaIngresoDetalle.Procesado
                });

            // Motivo 02: devolución.
            // Motivo 01: ajuste, no mueve stock.
            if (descuentaStock)
            {
                // Costo con el que sale: el de la línea de la nota referenciada, neto y en soles
                // (el legacy valoriza la devolución como ingreso negativo a ese precio).
                var precioSolesNc = NotaIngresoRules.ConvertPrices(
                    note.MonedaCode, precio, note.TipoCambio).Soles;
                var costoNetoNc = Math.Round(
                    precioSolesNc - (precioSolesNc * (descuento / 100m)),
                    6, MidpointRounding.AwayFromZero);

                var resultado = await _stock.ApplyAsync(
                    planta,
                    articulo,
                    cantidad,
                    0m,
                    0m,
                    0m,
                    increment: false,
                    recalculateAverage: false,
                    context: new StockMovementContext(
                        OrigenMovimientoArticulo.NotaCredito,
                        DocumentoCode: null, // se asigna al generar el correlativo de la nota
                        ItemNumber: item,
                        FechaMovimiento: request.FechaRecepcion,
                        CostoReferencia: costoNetoNc),
                    cancellationToken: ct);

                movimientos.Add(resultado.Movimiento);
            }
        }

        return note;
    }
}