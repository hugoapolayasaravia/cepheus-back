using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;

/// <summary>Armado y validación de las líneas (viguetas y bovedillas) de una Hoja de Corte.</summary>
public static class HojaCorteLineas
{
    public sealed record Resuelta(Producto Producto, string UnitCode);

    /// <summary>Código de producto (char(4)): si son solo dígitos se completa con ceros a la izquierda.</summary>
    public static string NormalizeProductoCode(string value)
    {
        var v = value.Trim();
        return v.Length > 0 && v.Length < 4 && v.All(char.IsDigit) ? v.PadLeft(4, '0') : v.ToUpperInvariant();
    }

    public static string NormalizeUnitCode(string value)
    {
        var v = value.Trim();
        return v.Length == 1 && char.IsDigit(v[0]) ? v.PadLeft(2, '0') : v.ToUpperInvariant();
    }

    /// <summary>
    /// Reglas de forma del PB: en viguetas el paño y la longitud son obligatorios; los cables del lado 2 solo
    /// se registran si hay cables en el lado 1. En bovedillas solo importan producto, unidad, cantidad y lotes.
    /// </summary>
    public static void EnsureShape(string tipo, HojaCorteDetalleFields f)
    {
        if (f.Quantity <= 0m)
        {
            throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
        }

        if (tipo == HojaCorteProductoTipos.Vigueta)
        {
            if (string.IsNullOrWhiteSpace(f.Panel))
            {
                throw new InvalidOperationException("El paño es obligatorio en una vigueta.");
            }

            if ((f.Length ?? 0m) <= 0m)
            {
                throw new InvalidOperationException("La longitud es obligatoria en una vigueta y debe ser mayor a cero.");
            }

            if ((f.CablesSide2 ?? 0m) > 0m && (f.CablesSide1 ?? 0m) <= 0m)
            {
                throw new InvalidOperationException(
                    "No se pueden registrar cables en el lado 2 si no hay cables en el lado 1.");
            }
        }
    }

    /// <summary>Verifica que el tipo de línea sea coherente con el TipoProducto de la cabecera.</summary>
    public static void EnsureConsistent(HojaCorte hc, string tipo)
    {
        if (hc.ProductType is null)
        {
            return;
        }

        var ok = hc.ProductType switch
        {
            TipoProductoHojaCorte.Vigueta => tipo == HojaCorteProductoTipos.Vigueta,
            TipoProductoHojaCorte.Bovedilla => tipo == HojaCorteProductoTipos.Bovedilla,
            _ => true
        };

        if (!ok)
        {
            throw new InvalidOperationException(
                $"La Hoja de Corte es de tipo de producto {hc.ProductType}; no admite líneas de " +
                (tipo == HojaCorteProductoTipos.Vigueta ? "viguetas." : "bovedillas."));
        }
    }

    public static async Task<Resuelta> ResolverAsync(
        IUnitOfWork uow,
        string tipo,
        string productoCode,
        string? unitCode,
        CancellationToken ct)
    {
        if (!HojaCorteProductoTipos.IsValid(tipo))
        {
            throw new InvalidOperationException(
                $"El tipo de producto {tipo} no es válido; use {HojaCorteProductoTipos.Vigueta} (vigueta) o {HojaCorteProductoTipos.Bovedilla} (bovedilla).");
        }

        var producto = await uow.Facturacion.Maestros.Productos
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.TipoProductoCode == tipo && x.Code == productoCode, ct)
            ?? throw new KeyNotFoundException($"El producto {tipo}/{productoCode} no existe.");

        if (!producto.IsActive)
        {
            throw new InvalidOperationException($"El producto {tipo}/{productoCode} está inactivo.");
        }

        var unit = string.IsNullOrWhiteSpace(unitCode) ? producto.UnitCode : NormalizeUnitCode(unitCode);

        var unitOk = await uow.Facturacion.Catalogos.UnidadesMedidaVenta
            .Query().AsNoTracking()
            .AnyAsync(x => x.Code == unit && x.IsActive, ct);

        if (!unitOk)
        {
            throw new KeyNotFoundException($"La unidad de medida {unit} no existe o está inactiva.");
        }

        return new Resuelta(producto, unit);
    }

    public static int NextItem(HojaCorte hc, string tipo)
        => hc.Detalles.Where(d => d.ProductoTipoCode == tipo).Select(d => d.ItemNumber).DefaultIfEmpty(0).Max() + 1;

    /// <summary>Copia los campos editables a la línea (sirve para alta y modificación).</summary>
    public static void Fill(HojaCorteDetalle linea, string tipo, string unitCode, HojaCorteDetalleFields f)
    {
        linea.UnitCode = unitCode;
        linea.Quantity = f.Quantity;
        linea.Lot1 = HojaCorteRules.NormalizeOrNull(f.Lot1);
        linea.Lot2 = HojaCorteRules.NormalizeOrNull(f.Lot2);
        linea.Lot3 = HojaCorteRules.NormalizeOrNull(f.Lot3);

        if (tipo == HojaCorteProductoTipos.Vigueta)
        {
            linea.Panel = HojaCorteRules.Normalize(f.Panel!);
            linea.Length = f.Length;
            linea.CablesSide1 = f.CablesSide1;
            linea.CablesSide2 = (f.CablesSide1 ?? 0m) > 0m ? f.CablesSide2 : null;
        }
        else
        {
            linea.Panel = null;
            linea.Length = null;
            linea.CablesSide1 = null;
            linea.CablesSide2 = null;
        }
    }

    public static HojaCorteDetalle Build(
        HojaCorte hc,
        string tipo,
        string productoCode,
        int item,
        string unitCode,
        HojaCorteDetalleFields f)
    {
        var linea = new HojaCorteDetalle
        {
            Year = hc.Year,
            HojaCorteCode = hc.Code,
            ProductoTipoCode = tipo,
            ProductoCode = productoCode,
            ItemNumber = item
        };

        Fill(linea, tipo, unitCode, f);
        return linea;
    }
}
