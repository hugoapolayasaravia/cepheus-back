using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>Contexto necesario para agregar, modificar o quitar líneas de una OT.</summary>
public sealed class LineContext
{
    public required OrdenTrabajoVenta Ot { get; init; }
    public CotizacionScope? Cotizacion { get; init; }
    public bool HasHojasCorte { get; init; }

    /// <summary>Negocios de las líneas de la OT de anticipo (solo OT tipo Descuento).</summary>
    public HashSet<string>? OriginNegocios { get; init; }
}

public static class OrdenTrabajoVentaLineas
{
    public static string NormalizeTipo(string value) => OrdenTrabajoVentaRules.NormalizeFixed(value, 2);

    public static string NormalizeProducto(string value) => OrdenTrabajoVentaRules.NormalizeFixed(value, 4);

    public static async Task<LineContext> BuildContextAsync(IUnitOfWork uow, OrdenTrabajoVenta ot, CancellationToken ct)
    {
        HashSet<string>? originNegocios = null;

        if (ot.Type == TipoOrdenTrabajoVenta.Descuento && ot.OriginCode is not null)
        {
            var list = await uow.Facturacion.Transacciones.OrdenesTrabajoVentaDetalle
                .Query().AsNoTracking()
                .Where(d => d.OrdenTrabajoCode == ot.OriginCode)
                .Select(d => d.NegocioCode)
                .Distinct()
                .ToListAsync(ct);

            originNegocios = list.ToHashSet(StringComparer.Ordinal);
        }

        return new LineContext
        {
            Ot = ot,
            Cotizacion = await OrdenTrabajoVentaCotizacion.LoadAsync(uow, ot, ct),
            HasHojasCorte = await OrdenTrabajoVentaRules.HasHojasCorteAsync(uow, ot.Code, ct),
            OriginNegocios = originNegocios
        };
    }

    /// <summary>Valida y agrega una línea a la OT (tracked). Devuelve la línea creada.</summary>
    public static async Task<OrdenTrabajoVentaDetalle> AddAsync(
        IUnitOfWork uow,
        LineContext ctx,
        OrdenTrabajoVentaDetalleRequest r,
        CancellationToken ct)
    {
        var ot = ctx.Ot;
        var tipo = NormalizeTipo(r.ProductoTipoCode);
        var productoCode = NormalizeProducto(r.ProductoCode);
        var negocio = OrdenTrabajoVentaRules.Normalize(r.NegocioCode);

        EnsureAmounts(r.Quantity, r.UnitPrice);

        var producto = await uow.Facturacion.Maestros.Productos
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.TipoProductoCode == tipo && x.Code == productoCode, ct)
            ?? throw new KeyNotFoundException($"El producto {tipo}/{productoCode} no existe.");

        if (!producto.IsActive)
        {
            throw new InvalidOperationException($"El producto {tipo}/{productoCode} está inactivo.");
        }

        var negocioOk = await uow.Comunes.Negocios
            .Query().AsNoTracking()
            .AnyAsync(n => n.Code == negocio && n.IsActive, ct);

        if (!negocioOk)
        {
            throw new KeyNotFoundException($"El negocio {negocio} no existe o está inactivo.");
        }

        if (negocio == OrdenTrabajoVentaRules.NegocioConcreto && ctx.Cotizacion is null)
        {
            throw new InvalidOperationException("El negocio de Concreto (CO) requiere que la OT provenga de una Cotización.");
        }

        if (ctx.HasHojasCorte && OrdenTrabajoVentaRules.IsHojaCorteProducto(tipo))
        {
            throw new InvalidOperationException(
                "La OT tiene Hojas de Corte: los productos de vigueta/bovedilla provienen de ellas y no se agregan a mano.");
        }

        if (ctx.OriginNegocios is not null && !ctx.OriginNegocios.Contains(negocio))
        {
            throw new InvalidOperationException(
                $"La OT Descuento debe ser del mismo negocio que la OT de Anticipo {ot.OriginCode}; el negocio {negocio} no está en ella.");
        }

        var price = r.UnitPrice;

        if (ctx.Cotizacion is not null && negocio == ctx.Cotizacion.Cotizacion.NegocioCode)
        {
            var cotPrice = ctx.Cotizacion.Consume(tipo, productoCode, r.Quantity);
            EnsurePriceNotAbove(r.UnitPrice, cotPrice, tipo, productoCode);
        }
        else if (ctx.Cotizacion is not null && negocio != OrdenTrabajoVentaRules.NegocioPretensados)
        {
            throw new InvalidOperationException(
                $"La OT proviene de la Cotización {ctx.Cotizacion.Describe()}: las líneas deben ser del negocio {ctx.Cotizacion.Cotizacion.NegocioCode} (o Pretensados).");
        }

        var signed = OrdenTrabajoVentaRules.SignedPrice(tipo, price);

        // PB: si ya existe una línea del mismo producto se suma la cantidad en vez de crear otra.
        var existing = ot.Detalles.FirstOrDefault(d => d.Status != EstadoDetalleOrdenTrabajoVenta.Anulado
                                                       && d.ProductoTipoCode == tipo
                                                       && d.ProductoCode == productoCode
                                                       && d.NegocioCode == negocio);

        if (existing is not null)
        {
            existing.Quantity += r.Quantity;
            existing.UnitPrice = signed;
            existing.Total = OrdenTrabajoVentaRules.LineTotal(existing.Quantity, signed);
            return existing;
        }

        var line = new OrdenTrabajoVentaDetalle
        {
            OrdenTrabajoCode = ot.Code,
            ProductoTipoCode = tipo,
            ProductoCode = productoCode,
            NegocioCode = negocio,
            Quantity = r.Quantity,
            UnitPrice = signed,
            Total = OrdenTrabajoVentaRules.LineTotal(r.Quantity, signed),
            Status = EstadoDetalleOrdenTrabajoVenta.Pendiente
        };

        ot.Detalles.Add(line);
        return line;
    }

    /// <summary>PB: el precio de la OT puede ser menor o igual al de la cotización, nunca mayor.</summary>
    public static void EnsurePriceNotAbove(decimal requested, decimal cotizacionPrice, string tipo, string producto)
    {
        if (requested > cotizacionPrice)
        {
            throw new InvalidOperationException(FormattableString.Invariant(
                $"El precio {requested:0.00######} del producto {tipo}/{producto} supera el de la cotización ({cotizacionPrice:0.00######})."));
        }
    }

    /// <summary>Línea de crédito: se valida al grabar las líneas (PB: w_reg_lim_cre_ot).</summary>
    public static async Task EnsureCreditAsync(IUnitOfWork uow, OrdenTrabajoVenta ot, CancellationToken ct)
    {
        var (cliente, obra, _) = await OrdenTrabajoVentaAprobaciones.GetPaymentContextAsync(uow, ot, ct);
        await OrdenTrabajoVentaCredito.EnsureCoveredAsync(uow, ot, cliente, obra, ct);
    }

    public static void EnsureAmounts(decimal quantity, decimal unitPrice)
    {
        if (quantity <= 0m)
        {
            throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
        }

        if (unitPrice <= 0m)
        {
            throw new InvalidOperationException("El precio debe ser mayor a cero (en productos tipo 23 se guarda negativo automáticamente).");
        }
    }

    public static OrdenTrabajoVentaDetalle FindLine(OrdenTrabajoVenta ot, int item)
        => ot.Detalles.FirstOrDefault(d => d.Item == item)
           ?? throw new KeyNotFoundException($"La línea {item} no existe en la Orden de Trabajo {ot.Code}.");

    /// <summary>
    /// Saldo de una OT de Anticipo = su total - lo ya descontado por OT tipo Descuento no anuladas.
    /// Cálculo provisional (el legacy usa CRE_FN_GET_SALDO_OT_ANTICIPO, cuya lógica no se tiene).
    /// </summary>
    public static async Task<decimal> GetAnticipoBalanceAsync(
        IUnitOfWork uow,
        string anticipoCode,
        string? excludeCode,
        CancellationToken ct)
    {
        var anticipoTotal = await uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query().AsNoTracking()
            .Where(x => x.Code == anticipoCode)
            .Select(x => x.Total)
            .FirstOrDefaultAsync(ct);

        var used = await uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query().AsNoTracking()
            .Where(x => x.OriginCode == anticipoCode
                        && x.Type == TipoOrdenTrabajoVenta.Descuento
                        && x.Stage != EtapaOrdenTrabajoVenta.Anulada
                        && x.Code != excludeCode)
            .Select(x => (decimal?)x.Total)
            .SumAsync(ct) ?? 0m;

        return anticipoTotal - Math.Abs(used);
    }

    /// <summary>PB (fg_validar_ot_descuento_anticipo): el total de la OT Descuento no puede superar el saldo del anticipo.</summary>
    public static async Task EnsureAnticipoBalanceAsync(IUnitOfWork uow, OrdenTrabajoVenta ot, CancellationToken ct)
    {
        if (ot.Type != TipoOrdenTrabajoVenta.Descuento || ot.OriginCode is null)
        {
            return;
        }

        var balance = await GetAnticipoBalanceAsync(uow, ot.OriginCode, ot.Code, ct);

        if (Math.Abs(ot.Total) > balance)
        {
            throw new InvalidOperationException(FormattableString.Invariant(
                $"El total {Math.Abs(ot.Total):0.00} supera el saldo de la OT de Anticipo {ot.OriginCode} ({balance:0.00})."));
        }
    }
}
