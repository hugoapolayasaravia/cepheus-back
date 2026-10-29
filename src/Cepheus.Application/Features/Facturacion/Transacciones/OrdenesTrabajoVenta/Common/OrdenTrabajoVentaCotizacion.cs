using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>
/// Cotización de origen de una OT (tracked) con sus líneas, para consumir y devolver cantidades
/// (PB: update CotizacionDetalle set Cantidad_Ent = Cantidad_Ent +/- cantidad).
///
/// El DDL de PedidoVentaDetalle no guarda el ítem de la cotización: la línea de OT se empareja con la(s)
/// línea(s) de la cotización por tipo + código de producto, en orden de ítem. Si la cotización repite el
/// producto, se consume y se devuelve secuencialmente.
/// </summary>
public sealed class CotizacionScope
{
    public CotizacionScope(Cotizacion cotizacion) => Cotizacion = cotizacion;

    public Cotizacion Cotizacion { get; }

    private IEnumerable<CotizacionDetalle> Matching(string tipo, string producto)
        => Cotizacion.Detalles
            .Where(d => d.ProductoTipoCode == tipo && d.ProductoCode == producto)
            .OrderBy(d => d.Item);

    public bool HasProduct(string tipo, string producto) => Matching(tipo, producto).Any();

    public decimal Pending(string tipo, string producto)
        => Matching(tipo, producto).Sum(d => d.Quantity - d.DeliveredQuantity);

    /// <summary>
    /// Consume la cantidad pedida de la cotización y devuelve el precio unitario de la primera línea con saldo.
    /// </summary>
    public decimal Consume(string tipo, string producto, decimal quantity)
    {
        var lines = Matching(tipo, producto).ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException(
                $"El producto {tipo}/{producto} no está en la Cotización {Describe()}.");
        }

        var pending = lines.Sum(d => d.Quantity - d.DeliveredQuantity);

        if (quantity > pending)
        {
            throw new InvalidOperationException(FormattableString.Invariant(
                $"La cantidad {quantity:0.##} del producto {tipo}/{producto} supera el saldo por atender de la Cotización {Describe()} ({pending:0.##})."));
        }

        decimal? price = null;
        var remaining = quantity;

        foreach (var line in lines)
        {
            var free = line.Quantity - line.DeliveredQuantity;

            if (free <= 0m)
            {
                continue;
            }

            price ??= line.UnitPrice;

            var take = Math.Min(free, remaining);
            line.DeliveredQuantity += take;
            remaining -= take;

            if (remaining <= 0m)
            {
                break;
            }
        }

        return price ?? lines[0].UnitPrice;
    }

    /// <summary>Devuelve cantidad a la cotización (de la última línea hacia atrás).</summary>
    public void Return(string tipo, string producto, decimal quantity)
    {
        var remaining = quantity;

        foreach (var line in Matching(tipo, producto).Reverse())
        {
            if (remaining <= 0m)
            {
                break;
            }

            var give = Math.Min(line.DeliveredQuantity, remaining);
            line.DeliveredQuantity -= give;
            remaining -= give;
        }
    }

    public string Describe() => $"{Cotizacion.NegocioCode}/{Cotizacion.Year}/{Cotizacion.Month}/{Cotizacion.Code}";
}

public static class OrdenTrabajoVentaCotizacion
{
    public static async Task<CotizacionScope?> LoadAsync(
        IUnitOfWork uow,
        string? negocio,
        string? year,
        string? month,
        string? code,
        CancellationToken ct)
    {
        if (negocio is null || year is null || month is null || code is null)
        {
            return null;
        }

        var cot = await uow.Facturacion.Transacciones.Cotizaciones
            .Query()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(
                x => x.NegocioCode == negocio && x.Year == year && x.Month == month && x.Code == code,
                ct);

        if (cot is null)
        {
            throw new KeyNotFoundException($"La Cotización {negocio}/{year}/{month}/{code} no existe.");
        }

        return new CotizacionScope(cot);
    }

    public static Task<CotizacionScope?> LoadAsync(IUnitOfWork uow, OrdenTrabajoVenta ot, CancellationToken ct)
        => LoadAsync(uow, ot.CotizacionNegocioCode, ot.CotizacionYear, ot.CotizacionMonth, ot.CotizacionCode, ct);

    /// <summary>Cotización válida para generar una OT: aprobada/aceptada y del mismo cliente y obra.</summary>
    public static void EnsureUsable(Cotizacion cot, string clienteCode, string obraCode)
    {
        if (cot.Status != EstadoCotizacion.Aprobado && cot.Status != EstadoCotizacion.Aceptado)
        {
            throw new InvalidOperationException(
                $"La Cotización {cot.NegocioCode}/{cot.Year}/{cot.Month}/{cot.Code} está {cot.Status}; solo se genera una OT desde una Cotización Aprobada o Aceptada.");
        }

        if (cot.ClienteCode != clienteCode || cot.ObraCode != obraCode)
        {
            throw new InvalidOperationException(
                "La Cotización pertenece a otro cliente u obra; el cliente y la obra de la OT deben ser los de la Cotización.");
        }
    }
}
