// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GenerarNotaIngresoImportacion/GenerarNotaIngresoImportacionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GenerarNotaIngresoImportacion
{
    /// <summary>
    /// Un solo SaveChanges: Nota de Ingreso, líneas, stock (servicio central), estado de las líneas y de la
    /// importación se confirman juntos o ninguno. No reintenta ante conflicto de concurrencia: descarta lo rastreado
    /// y pide reintentar (el servicio central de stock cachea filas nuevas y un reintento automático podría duplicarlas).
    /// </summary>
    public class GenerarNotaIngresoImportacionCommandHandler
        : IRequestHandler<GenerarNotaIngresoImportacionCommand, NotaIngresoResponse>
    {
        private const string ComprobanteInvalido = "00";
        private const string MonedaSoles = "PEN";

        private readonly IUnitOfWork _uow;
        private readonly IArticuloStockMovementService _stock;

        public GenerarNotaIngresoImportacionCommandHandler(IUnitOfWork uow, IArticuloStockMovementService stock)
        {
            _uow = uow;
            _stock = stock;
        }

        public async Task<NotaIngresoResponse> Handle(GenerarNotaIngresoImportacionCommand request, CancellationToken ct)
        {
            var planta = NotaIngresoRules.Normalize(request.PlantaCode);
            var importacionCode = NotaIngresoRules.Normalize(request.ImportacionCode);
            var proveedorCode = NotaIngresoRules.Normalize(request.ProveedorCode);

            var comprobante = await _uow.Comunes.ComprobantesPago.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.ComprobantePagoCode.Trim().ToUpper(), ct);

            if (comprobante is null || !comprobante.IsActive || comprobante.Code == ComprobanteInvalido)
                throw new InvalidOperationException("El comprobante de pago no existe o no es válido.");

            var esGuia = comprobante.Code == NotaIngresoRules.ComprobanteGuia;
            var esNotaCredito = comprobante.Code == NotaIngresoRules.ComprobanteNotaCredito;
            var numeroDocumento = esGuia ? null : request.NumeroDocumento?.Trim();
            var numeroGuia = request.NumeroGuia?.Trim();
            var numeroReferencia = request.NumeroReferencia?.Trim();

            if (esNotaCredito && string.IsNullOrWhiteSpace(numeroReferencia))
                throw new InvalidOperationException("Ingrese la serie y el número de referencia.");
            if (esGuia && string.IsNullOrWhiteSpace(numeroGuia))
                throw new InvalidOperationException("Debe indicar el número de guía.");
            if (!esGuia && string.IsNullOrWhiteSpace(numeroDocumento))
                throw new InvalidOperationException("Debe indicar el número de documento.");

            var importacion = await _uow.Logistica.Transacciones.Importaciones.Query()
                .Include(i => i.Detalles).ThenInclude(d => d.Articulo)
                .Include(i => i.Gastos).ThenInclude(g => g.Articulos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(i => i.PlantaCode == planta && i.Code == importacionCode, ct)
                ?? throw new KeyNotFoundException($"Importación {planta}/{importacionCode} no encontrada.");

            // Legacy (ue_set_btn_generar): una importación Procesada no se vuelve a generar.
            if (importacion.Estado != EstadoImportacion.Pendiente)
                throw new InvalidOperationException(
                    $"La Importación está en estado '{importacion.Estado}' y no admite generar Nota de Ingreso.");

            var seleccion = request.ArticuloCodes?.Select(NotaIngresoRules.Normalize).ToHashSet();
            var lineas = importacion.Detalles
                .Where(d => d.ProveedorCode == proveedorCode
                            && d.Estado == EstadoImportacion.Pendiente
                            && (seleccion is null || seleccion.Contains(d.ArticuloCode)))
                .OrderBy(d => d.ArticuloCode)
                .ToList();

            if (lineas.Count == 0)
                throw new InvalidOperationException("No hay artículos pendientes de este proveedor para generar.");

            if (seleccion is not null)
            {
                var faltantes = seleccion.Except(lineas.Select(l => l.ArticuloCode)).ToList();
                if (faltantes.Count > 0)
                    throw new InvalidOperationException(
                        $"Artículos no pendientes o inexistentes para el proveedor {proveedorCode}: {string.Join(", ", faltantes)}.");
            }

            if (!esGuia)
            {
                var duplicado = await _uow.Logistica.Transacciones.NotaIngresos.Query()
                    .AsNoTracking()
                    .AnyAsync(x => x.ComprobantePagoCode == comprobante.Code
                                   && x.ProveedorCode == proveedorCode
                                   && x.NumeroDocumento == numeroDocumento
                                   && x.Estado != EstadoNotaIngreso.Anulado, ct);
                if (duplicado)
                    throw new InvalidOperationException(
                        "Ya existe una Nota de Ingreso con el mismo tipo de documento, proveedor y número.");
            }

            // Mismas validaciones de fechas y período cerrado que la Nota de Ingreso de compra.
            await NotaIngresoRules.ValidateDatesAsync(_uow, planta, request.FechaEmision, request.FechaRecepcion, ct);

            // Legacy: Generar ejecuta Logi_sp_Actualiza_Importacion antes de leer los precios.
            var igvPercentage = await ImportacionTotalsCalculator.GetIgvPercentageAsync(_uow, importacion.FechaPoliza, ct);
            ImportacionTotalsCalculator.Recalculate(importacion, igvPercentage);
            ImportacionProrrateoCalculator.Calculate(importacion, igvPercentage);

            var note = new NotaIngreso
            {
                PlantaCode = planta,
                Condicion = CondicionNotaIngreso.OrdenCompra,
                Origen = OrigenNotaIngreso.Importacion,
                ImportacionCode = importacion.Code,
                ComprobantePagoCode = comprobante.Code,
                NumeroDocumento = numeroDocumento,
                NumeroGuia = esNotaCredito ? null : numeroGuia,
                NumeroReferencia = esNotaCredito ? numeroReferencia : null,
                ComprobantePagoReferenciaCode = esNotaCredito
                    ? NotaIngresoRules.NormalizeOrNull(request.ComprobantePagoReferenciaCode)
                    : null,
                ProveedorCode = proveedorCode,
                MonedaCode = MonedaSoles,
                FormaPagoCode = NotaIngresoRules.Normalize(request.FormaPagoCode),
                TipoCambio = 0m,
                FechaEmision = request.FechaEmision,
                FechaDocumento = request.FechaEmision,
                FechaRecepcion = request.FechaRecepcion,
                FechaProceso = DateTime.Now,
                Estado = EstadoNotaIngreso.Procesado
            };

            note.Code = await NotaIngresoRules.NextCodeAsync(_uow, planta, ct);

            var totalesLinea = new List<decimal>();
            var item = 0;

            foreach (var linea in lineas)
            {
                // dw_con_importaciones_articulos_pen: Precio = round(round(Valor_Det, 6) / Cantidad, 6); Total = Valor_Det.
                var precio = Math.Round(Math.Round(linea.ValorDet, 6, MidpointRounding.AwayFromZero) / linea.Cantidad, 6, MidpointRounding.AwayFromZero);
                if (precio <= 0)
                    throw new InvalidOperationException(
                        $"El precio del artículo {linea.ArticuloCode} no puede ser cero. Revise los valores de la importación.");

                var total = Math.Round(linea.ValorDet, 2, MidpointRounding.AwayFromZero);
                totalesLinea.Add(total);
                item++;

                note.Detalles.Add(new NotaIngresoDetalle
                {
                    PlantaCode = planta,
                    NotaIngresoCode = note.Code,
                    ItemNumber = item,
                    ArticuloCode = linea.ArticuloCode,
                    Cantidad = linea.Cantidad,
                    Precio = precio,
                    Descuento = 0m,
                    Total = total,
                    Estado = EstadoNotaIngresoDetalle.Procesado
                });

                // Stock + costo promedio en el servicio central (sin SaveChanges). El costo en dólares usa
                // el tipo de cambio de la importación, igual que ConvertPrices para una nota en soles.
                var (precioSoles, precioDolares) = NotaIngresoRules.ConvertPrices(MonedaSoles, precio, importacion.TipoCambio);

                await _stock.ApplyAsync(
                    planta, linea.ArticuloCode, linea.Cantidad, precioSoles, precioDolares, 0m,
                    increment: true, recalculateAverage: true,
                    context: new StockMovementContext(
                        OrigenMovimientoArticulo.Importacion, note.Code, item, note.FechaRecepcion),
                    cancellationToken: ct);

                linea.Estado = EstadoImportacion.Procesado;
            }

            var totales = ImportacionNotaIngresoTotals.Compute(importacion, lineas, totalesLinea);
            note.Total = totales.Total;
            note.Igv = totales.Igv;
            note.IgvExterior = totales.IgvExterior;
            note.Monto = totales.Monto;
            note.NoGravable = 0m;
            note.Renta = 0m;
            note.Fonavi = 0m;
            note.Servicio = 0m;

            // La importación pasa a Procesada cuando ya no queda ninguna línea Pendiente (generación parcial por proveedor).
            if (importacion.Detalles.All(d => d.Estado == EstadoImportacion.Procesado))
                importacion.Estado = EstadoImportacion.Procesado;

            await _uow.Logistica.Transacciones.NotaIngresos.AddAsync(note, ct);

            try
            {
                await _uow.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                _uow.ClearTracking();
                throw new InvalidOperationException(
                    "La importación o el stock fueron modificados por otro proceso. No se generó nada; intente nuevamente.");
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException(
                    $"Error al guardar la Nota de Ingreso: {ex.InnerException?.Message ?? ex.Message}", ex);
            }

            return await NotaIngresoReader.GetAsync(_uow, planta, note.Code, ct);
        }
    }
}
