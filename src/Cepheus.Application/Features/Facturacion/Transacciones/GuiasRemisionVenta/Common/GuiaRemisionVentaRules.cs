using System.Globalization;
using System.Text.RegularExpressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;

public static class GuiaRemisionVentaRules
{
    public const string DocEnEspera = "00";
    public const string DocFactura = "01";
    public const string DocBoleta = "03";

    /// <summary>COD_ACCION_DEV "01": la devolución vuelve al saldo de la OT (resta de lo entregado).</summary>
    public const string ActionReturnsToOrder = "01";

    public const string LocalCurrency = "PEN";

    private static readonly Regex TimeRegex = new(@"^([01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);
    private static readonly Regex TwoDigits = new(@"^\d{2}$", RegexOptions.Compiled);

    public static string NormalizeNumber(string value) => value.Trim().ToUpperInvariant();

    public static bool IsTwoDigits(string? value) => value is not null && TwoDigits.IsMatch(value);

    public static MotivoGuiaRemisionVenta ParseMotive(int value)
        => Enum.IsDefined(typeof(MotivoGuiaRemisionVenta), value)
            ? (MotivoGuiaRemisionVenta)value
            : throw new InvalidOperationException("Motivo inválido: use 1 (Venta), 2 (Traslado), 10 (Devolución) o 13 (Reposición).");

    public static TipoProgramacionGuia ParseProgramacionType(string value)
        => value.Trim().ToLowerInvariant() switch
        {
            "concreto" or "c" or "1" => TipoProgramacionGuia.Concreto,
            "otros" or "o" or "2" => TipoProgramacionGuia.Otros,
            _ => throw new InvalidOperationException("Tipo de programación inválido: use 'concreto' u 'otros'.")
        };

    /// <summary>PB fg_cierre: no se emite en un mes cerrado (TControlesVentas.Anomes_cierre &gt;= mes de la guía).</summary>
    public static async Task EnsurePeriodOpenAsync(IUnitOfWork uow, DateTime date, CancellationToken ct)
    {
        var control = await uow.Comunes.ControlesVentas.Query().AsNoTracking().FirstOrDefaultAsync(ct);

        if (control is null || string.IsNullOrWhiteSpace(control.ClosingPeriod))
        {
            return;
        }

        var month = date.ToString("yyyyMM", CultureInfo.InvariantCulture);

        if (string.CompareOrdinal(control.ClosingPeriod.Trim(), month) >= 0)
        {
            throw new InvalidOperationException($"El período {month} está cerrado: no se puede emitir ni modificar la guía.");
        }
    }

    /// <summary>PB fg_cambio 'V': tipo de cambio de venta del día; la moneda local vale 1.</summary>
    public static async Task<decimal> GetExchangeRateAsync(IUnitOfWork uow, string currency, DateTime date, CancellationToken ct)
    {
        var rate = await TryGetExchangeRateAsync(uow, currency, date, ct);

        return rate ?? throw new InvalidOperationException(
            $"No hay tipo de cambio de venta registrado para {currency} al {date:dd/MM/yyyy}.");
    }

    public static async Task<decimal?> TryGetExchangeRateAsync(IUnitOfWork uow, string currency, DateTime date, CancellationToken ct)
    {
        if (string.Equals(currency, LocalCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return 1m;
        }

        var day = DateOnly.FromDateTime(date);

        var row = await uow.Comunes.TiposCambio.Query().AsNoTracking()
            .Where(t => t.MonedaCode == currency && t.Date == day)
            .Select(t => (decimal?)t.SellRate)
            .FirstOrDefaultAsync(ct);

        return row;
    }

    public static string? TryNext(string last)
    {
        try
        {
            return OrdenTrabajoVentaDocumentos.Next(last);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public static async Task<Planta> LoadPlantaAsync(IUnitOfWork uow, string plantCode, bool tracked, CancellationToken ct)
    {
        var query = uow.Comunes.Plantas.Query();

        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        var planta = await query.FirstOrDefaultAsync(p => p.Code == plantCode, ct)
                     ?? throw new KeyNotFoundException($"La planta {plantCode} no existe.");

        if (!planta.IsActive)
        {
            throw new InvalidOperationException($"La planta {plantCode} está inactiva.");
        }

        return planta;
    }

    /// <summary>
    /// Número de la guía: el indicado (SSS-NNNNNN, no repetido) o el siguiente al último de la planta. Planta.NumeroGve
    /// solo avanza cuando el número tomado es exactamente último + 1 (PB). La planta debe estar tracked.
    /// </summary>
    public static async Task<string> TakeNumberAsync(IUnitOfWork uow, Planta planta, string? requested, CancellationToken ct)
    {
        var expected = TryNext(planta.NumeroGve);
        string number;

        if (string.IsNullOrWhiteSpace(requested))
        {
            number = expected ?? throw new InvalidOperationException(
                $"La numeración de guías de la planta ({planta.NumeroGve}) no tiene el formato SSS-NNNNNN; corríjala o indique el número.");
        }
        else
        {
            number = NormalizeNumber(requested);

            if (!OrdenTrabajoVentaDocumentos.IsValid(number))
            {
                throw new InvalidOperationException($"El número de guía '{number}' debe tener el formato SSS-NNNNNN.");
            }
        }

        if (await uow.Facturacion.Transacciones.GuiasRemisionVenta.Query().AsNoTracking().AnyAsync(g => g.Number == number, ct))
        {
            throw new InvalidOperationException($"La guía {number} ya existe.");
        }

        if (expected == number)
        {
            planta.NumeroGve = number;
        }

        return number;
    }

    public static async Task<OrdenTrabajoVenta> LoadOrderAsync(IUnitOfWork uow, string otCode, CancellationToken ct)
        => await uow.Facturacion.Transacciones.OrdenesTrabajoVenta
               .Query()
               .Include(o => o.Cliente)
               .Include(o => o.Obra)
               .Include(o => o.Aprobacion)
               .Include(o => o.Detalles).ThenInclude(d => d.Producto)
               .FirstOrDefaultAsync(o => o.Code == otCode, ct)
           ?? throw new KeyNotFoundException($"La Orden de Trabajo {otCode} no existe.");

    /// <summary>OT para devolver: Activa o Cerrada por entrega total (la devolución la reabre), con aprobación.</summary>
    public static void EnsureReturnable(OrdenTrabajoVenta ot)
    {
        if (ot.Status is not (EstadoOrdenTrabajoVenta.Activo or EstadoOrdenTrabajoVenta.Cerrado))
        {
            throw new InvalidOperationException($"La Orden de Trabajo está {ot.Status}; no admite devoluciones.");
        }

        if (ot.Aprobacion is null)
        {
            throw new InvalidOperationException("La Orden de Trabajo no tiene aprobación.");
        }
    }

    public static async Task<Obra> LoadObraAsync(IUnitOfWork uow, string clienteCode, string obraCode, CancellationToken ct)
    {
        var obra = await uow.Facturacion.Maestros.Obras.Query().AsNoTracking()
                       .Include(o => o.Cliente)
                       .FirstOrDefaultAsync(o => o.ClienteCode == clienteCode && o.Code == obraCode, ct)
                   ?? throw new KeyNotFoundException($"La obra {clienteCode}/{obraCode} no existe.");

        if (obra.Cliente.Estado is EstadoCliente.Inactivo or EstadoCliente.Suspendido)
        {
            throw new InvalidOperationException($"El cliente {clienteCode} está {obra.Cliente.Estado}.");
        }

        if (obra.Estado != EstadoObra.Activo)
        {
            throw new InvalidOperationException($"La obra {clienteCode}/{obraCode} está {obra.Estado}.");
        }

        return obra;
    }

    /// <summary>Transporte, destino y flete (campos obligatorios del PB: chofer, descripción, flete, dirección, ubigeo).</summary>
    public static async Task ApplyFieldsAsync(
        IUnitOfWork uow,
        GuiaRemisionVenta g,
        GuiaRemisionVentaFields f,
        Obra obra,
        CancellationToken ct)
    {
        var transportistaCode = OrdenTrabajoVentaRules.NormalizeFixed(f.TransportistaCode, 4);
        var choferCode = OrdenTrabajoVentaRules.NormalizeFixed(f.ChoferCode, 4);

        var transportista = await uow.Facturacion.Maestros.Transportistas.Query().AsNoTracking()
                                .FirstOrDefaultAsync(t => t.Code == transportistaCode, ct)
                            ?? throw new KeyNotFoundException($"El transportista {transportistaCode} no existe.");

        if (!transportista.IsActive)
        {
            throw new InvalidOperationException($"El transportista {transportistaCode} está inactivo.");
        }

        var chofer = await uow.Facturacion.Maestros.Choferes.Query().AsNoTracking()
                         .FirstOrDefaultAsync(c => c.TransportistaCode == transportistaCode && c.Code == choferCode, ct)
                     ?? throw new KeyNotFoundException($"El chofer {choferCode} no existe en el transportista {transportistaCode}.");

        if (!chofer.IsActive)
        {
            throw new InvalidOperationException($"El chofer {choferCode} está inactivo.");
        }

        TipoVehiculo? vehicleType = null;
        string? vehicleCode = null;

        if (f.VehicleType.HasValue || !string.IsNullOrWhiteSpace(f.VehicleCode))
        {
            if (!f.VehicleType.HasValue || string.IsNullOrWhiteSpace(f.VehicleCode))
            {
                throw new InvalidOperationException("Indique tipo y código del vehículo.");
            }

            if (!Enum.IsDefined(typeof(TipoVehiculo), f.VehicleType.Value))
            {
                throw new InvalidOperationException("Tipo de vehículo inválido: use 1 Camión, 2 Bomba/Mixer, 3 Carreta o 4 Otros.");
            }

            vehicleType = (TipoVehiculo)f.VehicleType.Value;
            vehicleCode = OrdenTrabajoVentaRules.NormalizeFixed(f.VehicleCode, 4);

            var vehicle = await uow.Facturacion.Maestros.Vehiculos.Query().AsNoTracking()
                              .FirstOrDefaultAsync(v => v.TransportistaCode == transportistaCode
                                                        && v.VehicleType == vehicleType && v.Code == vehicleCode, ct)
                          ?? throw new KeyNotFoundException($"El vehículo {vehicleType}/{vehicleCode} no existe en el transportista {transportistaCode}.");

            if (!vehicle.IsActive)
            {
                throw new InvalidOperationException($"El vehículo {vehicleCode} está inactivo.");
            }
        }

        var fleteCode = OrdenTrabajoVentaRules.NormalizeFixed(f.FleteCode, 2);

        var flete = await uow.Facturacion.Catalogos.Fletes.Query().AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Code == fleteCode, ct)
                    ?? throw new KeyNotFoundException($"El flete {fleteCode} no existe.");

        if (!flete.IsActive)
        {
            throw new InvalidOperationException($"El flete {fleteCode} está inactivo.");
        }

        if (f.FleteAmount is < 0m)
        {
            throw new InvalidOperationException("El monto del flete no puede ser negativo.");
        }

        var address = OrdenTrabajoVentaRules.NormalizeText(f.Address)
                      ?? (string.IsNullOrWhiteSpace(obra.DeliveryAddress) ? obra.Address : obra.DeliveryAddress);

        var ubigeoCode = string.IsNullOrWhiteSpace(f.UbigeoCode)
            ? (string.IsNullOrWhiteSpace(obra.DeliveryUbigeoCode) ? obra.UbigeoCode : obra.DeliveryUbigeoCode)
            : OrdenTrabajoVentaRules.NormalizeFixed(f.UbigeoCode, 6);

        if (string.IsNullOrWhiteSpace(address))
        {
            throw new InvalidOperationException("La dirección es obligatoria.");
        }

        if (!await uow.Comunes.Ubigeos.Query().AsNoTracking().AnyAsync(u => u.Code == ubigeoCode, ct))
        {
            throw new KeyNotFoundException($"El ubigeo {ubigeoCode} no existe.");
        }

        var description = OrdenTrabajoVentaRules.NormalizeText(f.Description)
                          ?? throw new InvalidOperationException("La descripción es obligatoria.");

        var time = string.IsNullOrWhiteSpace(f.DispatchTime) ? null : f.DispatchTime.Trim();

        if (time is not null && !TimeRegex.IsMatch(time))
        {
            throw new InvalidOperationException("La hora de despacho no es válida (HH:mm).");
        }

        g.TransportistaCode = transportistaCode;
        g.ChoferCode = choferCode;
        g.VehicleType = vehicleType;
        g.VehicleCode = vehicleCode;
        g.Address = address;
        g.UbigeoCode = ubigeoCode;
        g.Description = description;
        g.FleteCode = fleteCode;
        g.FleteAmount = f.FleteAmount ?? flete.Amount;
        g.DispatchTime = time;
        g.TicketNumber = OrdenTrabajoVentaRules.NormalizeText(f.TicketNumber);
        g.PumpCode = OrdenTrabajoVentaRules.NormalizeText(f.PumpCode);
    }

    /// <summary>Documento de pago que corresponde a la guía según la aprobación de la OT (F -> 01, B -> 03).</summary>
    public static void ApplyDocument(GuiaRemisionVenta g, OrdenTrabajoVenta? ot)
    {
        g.DocumentFlow = null;
        g.DocumentTypeCode = null;
        g.Document1Number = null;
        g.Document2Number = null;

        if (g.Motive is MotivoGuiaRemisionVenta.Traslado or MotivoGuiaRemisionVenta.Devolucion)
        {
            return;
        }

        if (ot?.Aprobacion is not { } a)
        {
            g.DocumentTypeCode = DocEnEspera;
            return;
        }

        g.DocumentFlow = a.DocumentFlow;
        g.DocumentTypeCode = a.DocumentType == TipoDocumentoAprobacion.Factura ? DocFactura : DocBoleta;

        // Al contado la aprobación ya trae el número; a crédito el documento lo asigna el comprobante (entregable 5).
        if (a.PaymentType == TipoPagoAprobacion.Contado)
        {
            g.Document1Number = a.Document1Number;
            g.Document2Number = a.Document2Number;
        }
    }

    /// <summary>Líneas con OT (programación para venta/reposición; entregado de la OT para devolución).</summary>
    public static List<GuiaRemisionVentaDetalle> BuildOrderLines(
        GuiaRemisionVenta g,
        OrdenTrabajoVenta ot,
        GuiaProgramacionInfo? prog,
        IReadOnlyList<GuiaRemisionVentaDetalleRequest> requests,
        IReadOnlyDictionary<int, decimal> ownOld)
    {
        if (requests.Count == 0)
        {
            throw new InvalidOperationException("La guía debe tener al menos una línea.");
        }

        var lines = new List<GuiaRemisionVentaDetalle>();

        foreach (var r in requests)
        {
            var item = r.Item ?? throw new InvalidOperationException("Cada línea debe indicar el Item de la OT.");

            var otLine = ot.Detalles.FirstOrDefault(d => d.Item == item)
                         ?? throw new InvalidOperationException($"La línea {item} no pertenece a la Orden de Trabajo {ot.Code}.");

            if (otLine.Status == EstadoDetalleOrdenTrabajoVenta.Anulado)
            {
                throw new InvalidOperationException($"La línea {item} de la OT está anulada.");
            }

            if (r.Quantity <= 0m)
            {
                throw new InvalidOperationException($"{otLine.Producto.Name}: la cantidad debe ser mayor a cero.");
            }

            var own = ownOld.GetValueOrDefault(item);
            decimal? tope = null;

            if (g.Motive == MotivoGuiaRemisionVenta.Devolucion)
            {
                var returnable = otLine.DeliveredQuantity + (g.DevolutionActionCode == ActionReturnsToOrder ? own : 0m);

                if (r.Quantity > returnable)
                {
                    throw new InvalidOperationException(FormattableString.Invariant(
                        $"{otLine.Producto.Name}: la devolución ({r.Quantity:0.##}) es mayor a lo entregado ({returnable:0.##})."));
                }
            }
            else
            {
                var pl = prog!.Lines.FirstOrDefault(l => l.Item == item)
                         ?? throw new InvalidOperationException($"La línea {item} no pertenece a la programación {prog.Code}.");

                tope = pl.Quantity - (pl.Delivered - own);

                if (r.Quantity > tope)
                {
                    throw new InvalidOperationException(FormattableString.Invariant(
                        $"{otLine.Producto.Name}: Cantidad Mayor a lo Programado (pendiente {tope:0.##})."));
                }

                var balance = otLine.ApprovedQuantity - (otLine.DeliveredQuantity - own);

                if (r.Quantity > balance)
                {
                    throw new InvalidOperationException(FormattableString.Invariant(
                        $"{otLine.Producto.Name}: cantidad mayor al saldo aprobado de la OT ({balance:0.##})."));
                }
            }

            lines.Add(new GuiaRemisionVentaDetalle
            {
                GuiaNumber = g.Number,
                Line = lines.Count + 1,
                OrdenTrabajoCode = ot.Code,
                OrdenTrabajoItem = item,
                ProductoTipoCode = otLine.ProductoTipoCode,
                ProductoCode = otLine.ProductoCode,
                Quantity = r.Quantity,
                TopeQuantity = tope,
                UnitPrice = otLine.UnitPrice
            });
        }

        return lines;
    }

    /// <summary>Líneas de una guía sin OT: producto, cantidad y precio indicados.</summary>
    public static async Task<List<GuiaRemisionVentaDetalle>> BuildFreeLinesAsync(
        IUnitOfWork uow,
        GuiaRemisionVenta g,
        IReadOnlyList<GuiaRemisionVentaDetalleRequest> requests,
        CancellationToken ct)
    {
        if (requests.Count == 0)
        {
            throw new InvalidOperationException("La guía debe tener al menos una línea.");
        }

        var lines = new List<GuiaRemisionVentaDetalle>();

        foreach (var r in requests)
        {
            if (string.IsNullOrWhiteSpace(r.ProductoTipoCode) || string.IsNullOrWhiteSpace(r.ProductoCode))
            {
                throw new InvalidOperationException("Cada línea sin OT debe indicar tipo y código de producto.");
            }

            var tipo = OrdenTrabajoVentaRules.NormalizeTwo(r.ProductoTipoCode);
            var code = OrdenTrabajoVentaRules.NormalizeProductoCode(r.ProductoCode);

            if (!await uow.Facturacion.Maestros.Productos.Query().AsNoTracking()
                    .AnyAsync(p => p.TipoProductoCode == tipo && p.Code == code, ct))
            {
                throw new KeyNotFoundException($"El producto {tipo}/{code} no existe.");
            }

            if (r.Quantity <= 0m)
            {
                throw new InvalidOperationException($"Producto {tipo}/{code}: la cantidad debe ser mayor a cero.");
            }

            if (r.UnitPrice is < 0m)
            {
                throw new InvalidOperationException($"Producto {tipo}/{code}: el precio no puede ser negativo.");
            }

            lines.Add(new GuiaRemisionVentaDetalle
            {
                GuiaNumber = g.Number,
                Line = lines.Count + 1,
                ProductoTipoCode = tipo,
                ProductoCode = code,
                Quantity = r.Quantity,
                UnitPrice = r.UnitPrice ?? 0m
            });
        }

        return lines;
    }

    /// <summary>
    /// Reemplaza las líneas de la guía conservando las filas existentes por número de línea (evita borrar y volver a
    /// insertar la misma clave): actualiza, agrega las que sobran y quita las de más.
    /// </summary>
    public static void ReplaceLines(GuiaRemisionVenta g, IReadOnlyList<GuiaRemisionVentaDetalle> fresh)
    {
        var existing = g.Detalles.OrderBy(d => d.Line).ToList();

        for (var i = 0; i < fresh.Count; i++)
        {
            var n = fresh[i];

            if (i < existing.Count)
            {
                var e = existing[i];
                e.OrdenTrabajoCode = n.OrdenTrabajoCode;
                e.OrdenTrabajoItem = n.OrdenTrabajoItem;
                e.ProductoTipoCode = n.ProductoTipoCode;
                e.ProductoCode = n.ProductoCode;
                e.Quantity = n.Quantity;
                e.TopeQuantity = n.TopeQuantity;
                e.UnitPrice = n.UnitPrice;
            }
            else
            {
                g.Detalles.Add(n);
            }
        }

        for (var i = fresh.Count; i < existing.Count; i++)
        {
            g.Detalles.Remove(existing[i]);
        }
    }

    public static async Task SaveAsync(IUnitOfWork uow, string what, CancellationToken ct)
    {
        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Error al guardar {what}: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }
}
