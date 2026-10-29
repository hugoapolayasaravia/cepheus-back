using System.Globalization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;

public static class ComprobanteVentaRules
{
    public const string Factura = "01";
    public const string Boleta = "03";

    /// <summary>SUNAT catálogo 06: documento de identidad RUC.</summary>
    public const string RucSunatCode = "6";

    public static string NormalizeType(string? value)
        => value?.Trim() switch
        {
            "01" or "1" => Factura,
            "03" or "3" => Boleta,
            _ => throw new InvalidOperationException("Tipo de comprobante inválido: use 01 (Factura) o 03 (Boleta).")
        };

    public static string NormalizeNumber(string value) => value.Trim().ToUpperInvariant();

    public static TipoDocumentoAprobacion ToApprovalType(string typeCode)
        => typeCode == Factura ? TipoDocumentoAprobacion.Factura : TipoDocumentoAprobacion.Boleta;

    public static string TypeName(string typeCode) => typeCode == Factura ? "Factura" : "Boleta";

    /// <summary>PB: la fecha de emisión no puede ser mayor a la de proceso (hoy).</summary>
    public static void EnsureIssueDate(DateTime date)
    {
        if (date.Date > DateTime.Today)
        {
            throw new InvalidOperationException("La fecha de emisión no puede ser mayor a la fecha del sistema.");
        }
    }

    /// <summary>PB (solo factura): el cliente debe registrar RUC.</summary>
    public static void EnsureRuc(string typeCode, Cliente cliente)
    {
        if (typeCode != Factura)
        {
            return;
        }

        if (cliente.DocumentType?.SunatCode != RucSunatCode || string.IsNullOrWhiteSpace(cliente.DocumentNumber))
        {
            throw new InvalidOperationException($"El cliente {cliente.Code} no registra N° de RUC: no se puede emitir factura.");
        }
    }

    /// <summary>fn_Igv: porcentaje vigente a la fecha (TPorcentajeIgv); si no hay periodo, el de ControlVentas.</summary>
    public static async Task<decimal> GetIgvPercentageAsync(IUnitOfWork uow, DateTime date, CancellationToken ct)
    {
        var day = DateOnly.FromDateTime(date);

        var pct = await uow.Comunes.PeriodosIgv.Query().AsNoTracking()
            .Where(p => p.FechaInicio <= day && (p.FechaFin == null || p.FechaFin >= day))
            .OrderByDescending(p => p.FechaInicio)
            .Select(p => (decimal?)p.PorcentajeIgv)
            .FirstOrDefaultAsync(ct);

        if (pct.HasValue)
        {
            return pct.Value;
        }

        var control = await uow.Comunes.ControlesVentas.Query().AsNoTracking().FirstOrDefaultAsync(ct);

        return control?.IgvPercentage
               ?? throw new InvalidOperationException($"No hay porcentaje de IGV registrado para el {date:dd/MM/yyyy}.");
    }

    /// <summary>
    /// Totales (ad_act_tcambio_factura): total de línea = redondeo(cantidad x precio, 2); subtotal = suma;
    /// IGV = redondeo(subtotal x % / 100, 2); total = subtotal + IGV; equivalente en soles con el tipo de cambio.
    /// </summary>
    public static void Recalc(ComprobanteVenta c)
    {
        foreach (var l in c.Detalles)
        {
            l.Total = Math.Round(l.Quantity * l.UnitPrice, 2, MidpointRounding.AwayFromZero);
        }

        c.SubtotalAmount = c.Detalles.Sum(l => l.Total);
        c.IgvAmount = Math.Round(c.SubtotalAmount * c.IgvPercentage / 100m, 2, MidpointRounding.AwayFromZero);
        c.TotalAmount = c.SubtotalAmount + c.IgvAmount;

        c.TotalSoles = string.Equals(c.CurrencyCode, GuiaRemisionVentaRules.LocalCurrency, StringComparison.OrdinalIgnoreCase)
            ? c.TotalAmount
            : Math.Round(c.TotalAmount * c.ExchangeRate, 2, MidpointRounding.AwayFromZero);
    }

    public static async Task<Cliente> LoadClienteAsync(IUnitOfWork uow, string clienteCode, CancellationToken ct)
        => await uow.Facturacion.Maestros.Clientes.Query().AsNoTracking()
               .Include(c => c.DocumentType)
               .FirstOrDefaultAsync(c => c.Code == clienteCode, ct)
           ?? throw new KeyNotFoundException($"El cliente {clienteCode} no existe.");

    public static async Task<ComprobanteVenta> LoadForUpdateAsync(IUnitOfWork uow, string typeCode, string number, CancellationToken ct)
        => await uow.Facturacion.Transacciones.ComprobantesVenta
               .Query()
               .Include(x => x.Detalles)
               .FirstOrDefaultAsync(x => x.TypeCode == typeCode && x.Number == number, ct)
           ?? throw new KeyNotFoundException($"El comprobante {TypeName(typeCode)} {number} no existe.");

    /// <summary>
    /// Número del comprobante: el de las guías (si ya lo traen), el indicado o el siguiente al último de la planta.
    /// La numeración de la planta avanza solo cuando el número es exactamente último + 1 (PB). Planta tracked.
    /// </summary>
    public static async Task<string> TakeNumberAsync(
        IUnitOfWork uow,
        Planta planta,
        string typeCode,
        string? fixedNumber,
        string? requested,
        CancellationToken ct)
    {
        var approvalType = ToApprovalType(typeCode);
        var last = OrdenTrabajoVentaDocumentos.GetLast(planta, approvalType);
        var expected = GuiaRemisionVentaRules.TryNext(last);
        string number;

        if (!string.IsNullOrWhiteSpace(fixedNumber))
        {
            number = fixedNumber;

            if (!string.IsNullOrWhiteSpace(requested) && NormalizeNumber(requested) != number)
            {
                throw new InvalidOperationException(
                    $"Las guías ya tienen asignado el documento {number}; no se puede emitir con el número {NormalizeNumber(requested)}.");
            }
        }
        else if (!string.IsNullOrWhiteSpace(requested))
        {
            number = NormalizeNumber(requested);
        }
        else
        {
            number = expected ?? throw new InvalidOperationException(
                $"La numeración de la planta ({last}) no tiene el formato SSS-NNNNNN; corríjala o indique el número.");
        }

        if (!OrdenTrabajoVentaDocumentos.IsValid(number))
        {
            throw new InvalidOperationException($"El número '{number}' debe tener el formato SSS-NNNNNN.");
        }

        if (await uow.Facturacion.Transacciones.ComprobantesVenta.Query().AsNoTracking()
                .AnyAsync(c => c.TypeCode == typeCode && c.Number == number, ct))
        {
            throw new InvalidOperationException($"El comprobante {TypeName(typeCode)} {number} ya está registrado.");
        }

        if (expected == number)
        {
            OrdenTrabajoVentaDocumentos.SetLast(planta, approvalType, number);
        }

        return number;
    }

    /// <summary>
    /// Agrega al comprobante las líneas de las guías y las marca Facturado con el documento. Las guías deben ser
    /// Pendientes, de venta, del mismo cliente, obra, OT y moneda del comprobante, y de su tipo de documento.
    /// </summary>
    public static void AttachGuias(ComprobanteVenta c, IReadOnlyCollection<GuiaRemisionVenta> guias)
    {
        foreach (var g in guias.OrderBy(x => x.IssueDate).ThenBy(x => x.Number))
        {
            if (g.Status != EstadoGuiaRemisionVenta.Pendiente)
            {
                throw new InvalidOperationException($"La guía {g.Number} está {g.Status}; solo se facturan guías Pendientes.");
            }

            if (g.Motive != MotivoGuiaRemisionVenta.Venta)
            {
                throw new InvalidOperationException($"La guía {g.Number} es de {g.Motive}; solo se facturan guías de Venta.");
            }

            if (g.DocumentTypeCode is not null
                && g.DocumentTypeCode != GuiaRemisionVentaRules.DocEnEspera
                && g.DocumentTypeCode != c.TypeCode)
            {
                throw new InvalidOperationException(
                    $"La guía {g.Number} corresponde a {(g.DocumentTypeCode == Factura ? "Factura" : "Boleta")}.");
            }

            if (g.Document1Number is not null && g.Document1Number != c.Number)
            {
                throw new InvalidOperationException($"La guía {g.Number} ya tiene asignado el documento {g.Document1Number}.");
            }

            if (g.ClienteCode != c.ClienteCode || g.ObraCode != c.ObraCode)
            {
                throw new InvalidOperationException($"La guía {g.Number} es de otro cliente u obra.");
            }

            if (g.OrdenTrabajoCode != c.OrdenTrabajoCode)
            {
                throw new InvalidOperationException($"La guía {g.Number} es de otra Orden de Trabajo.");
            }

            if (g.CurrencyCode != c.CurrencyCode)
            {
                throw new InvalidOperationException($"La guía {g.Number} está en {g.CurrencyCode}; el comprobante en {c.CurrencyCode}.");
            }

            if (g.Detalles.Count == 0)
            {
                throw new InvalidOperationException($"La guía {g.Number} no tiene detalle.");
            }

            var line = c.Detalles.Count == 0 ? 0 : c.Detalles.Max(d => d.Line);

            foreach (var d in g.Detalles.OrderBy(x => x.Line))
            {
                if (d.UnitPrice <= 0m)
                {
                    throw new InvalidOperationException($"La guía {g.Number}, línea {d.Line}: no tiene precio.");
                }

                c.Detalles.Add(new ComprobanteVentaDetalle
                {
                    TypeCode = c.TypeCode,
                    Number = c.Number,
                    Line = ++line,
                    GuiaNumber = g.Number,
                    GuiaLine = d.Line,
                    OrdenTrabajoCode = d.OrdenTrabajoCode,
                    OrdenTrabajoItem = d.OrdenTrabajoItem,
                    ProductoTipoCode = d.ProductoTipoCode,
                    ProductoCode = d.ProductoCode,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                });
            }

            g.Status = EstadoGuiaRemisionVenta.Facturado;
            g.DocumentTypeCode = c.TypeCode;
            g.Document1Number = c.Number;
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

    public static string Money(decimal v) => v.ToString("0.00", CultureInfo.InvariantCulture);
}
