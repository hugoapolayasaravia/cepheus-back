using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;

/// <summary>Líneas de un comprobante libre (producto, cantidad y precio; PB: tipo 'O').</summary>
public static class ComprobanteVentaLibreLines
{
    public static async Task<List<ComprobanteVentaDetalle>> BuildAsync(
        IUnitOfWork uow,
        ComprobanteVenta c,
        IReadOnlyList<ComprobanteVentaLineaLibreRequest> requests,
        CancellationToken ct)
    {
        if (requests.Count == 0)
        {
            throw new InvalidOperationException("El comprobante debe tener al menos una línea.");
        }

        var lines = new List<ComprobanteVentaDetalle>();

        foreach (var r in requests)
        {
            var tipo = OrdenTrabajoVentaRules.NormalizeTwo(r.ProductoTipoCode);
            var code = OrdenTrabajoVentaRules.NormalizeProductoCode(r.ProductoCode);

            if (!await uow.Facturacion.Maestros.Productos.Query().AsNoTracking()
                    .AnyAsync(p => p.TipoProductoCode == tipo && p.Code == code, ct))
            {
                throw new KeyNotFoundException($"El producto {tipo}/{code} no existe.");
            }

            if (r.Quantity <= 0m)
            {
                throw new InvalidOperationException($"Producto {tipo}/{code}: cantidad errada.");
            }

            if (r.UnitPrice <= 0m)
            {
                throw new InvalidOperationException($"Producto {tipo}/{code}: el precio no puede estar en blanco.");
            }

            lines.Add(new ComprobanteVentaDetalle
            {
                TypeCode = c.TypeCode,
                Number = c.Number,
                Line = lines.Count + 1,
                ProductoTipoCode = tipo,
                ProductoCode = code,
                Quantity = r.Quantity,
                UnitPrice = r.UnitPrice
            });
        }

        return lines;
    }

    /// <summary>Reemplaza las líneas por número de línea (actualiza, agrega y quita) sin borrar y reinsertar la misma clave.</summary>
    public static void Replace(ComprobanteVenta c, IReadOnlyList<ComprobanteVentaDetalle> fresh)
    {
        var existing = c.Detalles.OrderBy(d => d.Line).ToList();

        for (var i = 0; i < fresh.Count; i++)
        {
            if (i < existing.Count)
            {
                var e = existing[i];
                e.ProductoTipoCode = fresh[i].ProductoTipoCode;
                e.ProductoCode = fresh[i].ProductoCode;
                e.Quantity = fresh[i].Quantity;
                e.UnitPrice = fresh[i].UnitPrice;
            }
            else
            {
                c.Detalles.Add(fresh[i]);
            }
        }

        for (var i = fresh.Count; i < existing.Count; i++)
        {
            c.Detalles.Remove(existing[i]);
        }
    }
}
