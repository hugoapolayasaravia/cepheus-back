using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;

public sealed class GuiaProgramacionLine
{
    public int Item { get; set; }
    public decimal Quantity { get; set; }
    public decimal Delivered { get; set; }
}

public sealed class GuiaProgramacionInfo
{
    public TipoProgramacionGuia Type { get; set; }
    public string Code { get; set; } = default!;
    public string OrdenTrabajoCode { get; set; } = default!;
    public string PlantCode { get; set; } = default!;
    public EstadoProgramacion Status { get; set; }
    public List<GuiaProgramacionLine> Lines { get; set; } = new();
}

/// <summary>Acceso a la programación de Concreto o de Otros desde la guía.</summary>
public static class GuiaRemisionVentaProgramaciones
{
    public static string NormalizeCode(string code) => code.Trim().PadLeft(8, '0');

    public static async Task<GuiaProgramacionInfo> LoadAsync(
        IUnitOfWork uow,
        TipoProgramacionGuia type,
        string code,
        CancellationToken ct)
    {
        code = NormalizeCode(code);

        if (type == TipoProgramacionGuia.Concreto)
        {
            var p = await uow.Facturacion.Transacciones.ProgramacionesConcreto
                        .Query().AsNoTracking()
                        .Include(x => x.Detalles)
                        .FirstOrDefaultAsync(x => x.Code == code, ct)
                    ?? throw new KeyNotFoundException($"La programación de Concreto {code} no existe.");

            return new GuiaProgramacionInfo
            {
                Type = type,
                Code = p.Code,
                OrdenTrabajoCode = p.OrdenTrabajoCode,
                PlantCode = p.PlantCode,
                Status = p.Status,
                Lines = p.Detalles
                    .Select(d => new GuiaProgramacionLine { Item = d.Item, Quantity = d.Quantity, Delivered = d.DeliveredQuantity })
                    .ToList()
            };
        }

        var o = await uow.Facturacion.Transacciones.ProgramacionesOtros
                    .Query().AsNoTracking()
                    .Include(x => x.Detalles)
                    .FirstOrDefaultAsync(x => x.Code == code, ct)
                ?? throw new KeyNotFoundException($"La programación de Otros {code} no existe.");

        return new GuiaProgramacionInfo
        {
            Type = type,
            Code = o.Code,
            OrdenTrabajoCode = o.OrdenTrabajoCode,
            PlantCode = o.PlantCode,
            Status = o.Status,
            Lines = o.Detalles
                .Select(d => new GuiaProgramacionLine { Item = d.Item, Quantity = d.Quantity, Delivered = d.DeliveredQuantity })
                .ToList()
        };
    }

    /// <summary>
    /// Fija lo entregado de cada línea de la programación y su estado (PB ad_actualizasaldos): Cerrada si no queda
    /// saldo pendiente, Pendiente si queda. Una programación Anulada no cambia de estado.
    /// </summary>
    public static async Task ApplyDeliveredAsync(
        IUnitOfWork uow,
        TipoProgramacionGuia type,
        string code,
        IReadOnlyDictionary<int, decimal> delivered,
        CancellationToken ct)
    {
        code = NormalizeCode(code);

        if (type == TipoProgramacionGuia.Concreto)
        {
            var p = await uow.Facturacion.Transacciones.ProgramacionesConcreto
                        .Query()
                        .Include(x => x.Detalles)
                        .FirstOrDefaultAsync(x => x.Code == code, ct)
                    ?? throw new KeyNotFoundException($"La programación de Concreto {code} no existe.");

            foreach (var d in p.Detalles)
            {
                d.DeliveredQuantity = delivered.GetValueOrDefault(d.Item);
            }

            if (p.Status != EstadoProgramacion.Anulada)
            {
                p.Status = p.Detalles.All(d => d.Quantity - d.DeliveredQuantity <= 0m)
                    ? EstadoProgramacion.Cerrada
                    : EstadoProgramacion.Pendiente;
            }

            return;
        }

        var o = await uow.Facturacion.Transacciones.ProgramacionesOtros
                    .Query()
                    .Include(x => x.Detalles)
                    .FirstOrDefaultAsync(x => x.Code == code, ct)
                ?? throw new KeyNotFoundException($"La programación de Otros {code} no existe.");

        foreach (var d in o.Detalles)
        {
            d.DeliveredQuantity = delivered.GetValueOrDefault(d.Item);
        }

        if (o.Status != EstadoProgramacion.Anulada)
        {
            o.Status = o.Detalles.All(d => d.Quantity - d.DeliveredQuantity <= 0m)
                ? EstadoProgramacion.Cerrada
                : EstadoProgramacion.Pendiente;
        }
    }
}
