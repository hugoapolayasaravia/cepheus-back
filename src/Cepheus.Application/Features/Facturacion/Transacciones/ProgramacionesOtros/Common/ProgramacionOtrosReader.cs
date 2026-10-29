using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;

public static class ProgramacionOtrosReader
{
    public static async Task<ProgramacionOtrosResponse> GetAsync(IUnitOfWork uow, string code, CancellationToken ct)
    {
        var p = await uow.Facturacion.Transacciones.ProgramacionesOtros
            .Query().AsNoTracking()
            .Include(x => x.Planta)
            .Include(x => x.Detalles)
            .Include(x => x.OrdenTrabajo).ThenInclude(o => o.Cliente)
            .Include(x => x.OrdenTrabajo).ThenInclude(o => o.Obra)
            .Include(x => x.OrdenTrabajo).ThenInclude(o => o.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(x => x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La programación de Otros {code} no existe.");

        var ot = p.OrdenTrabajo;

        var responsible = await uow.Facturacion.Catalogos.ResponsablesObra
            .Query().AsNoTracking()
            .Where(r => r.ClienteCode == ot.ClienteCode && r.ObraCode == ot.ObraCode)
            .OrderBy(r => r.Code)
            .FirstOrDefaultAsync(ct);

        var hasDeliveries = await ProgramacionOtrosRules.HasDeliveriesAsync(uow, p, ct);

        var lines = p.Detalles
            .OrderBy(d => d.Item)
            .Select(d =>
            {
                var l = ot.Detalles.First(x => x.Item == d.Item);

                return new ProgramacionOtrosDetalleResponse
                {
                    Item = d.Item,
                    NegocioCode = l.NegocioCode,
                    ProductoTipoCode = l.ProductoTipoCode,
                    ProductoCode = l.ProductoCode,
                    ProductoName = l.Producto.Name,
                    UnitCode = l.Producto.UnitCode,
                    Quantity = d.Quantity,
                    DeliveredQuantity = d.DeliveredQuantity,
                    BalanceQuantity = d.Quantity - d.DeliveredQuantity,
                    OrderQuantity = l.Quantity,
                    ApprovedQuantity = l.ApprovedQuantity,
                    AvailableQuantity = l.ApprovedQuantity - l.ProgrammedQuantity
                };
            })
            .ToList();

        return new ProgramacionOtrosResponse
        {
            Code = p.Code,
            OrdenTrabajoCode = p.OrdenTrabajoCode,
            ClienteCode = ot.ClienteCode,
            ClienteName = ot.Cliente.Name,
            ObraCode = ot.ObraCode,
            ObraName = ot.Obra.Description,
            DeliveryAddress = string.IsNullOrWhiteSpace(ot.Obra.DeliveryAddress) ? ot.Obra.Address : ot.Obra.DeliveryAddress,
            ResponsibleName = responsible?.Name,
            ResponsiblePhone = responsible?.Phone,
            VendedorCode = ot.VendedorCode,
            ProgramDate = p.ProgramDate,
            ProgramTime = p.ProgramTime,
            PlantCode = p.PlantCode,
            PlantName = p.Planta.Name,
            TransportObservations = p.TransportObservations,
            Status = p.Status,
            Quantity = lines.Sum(l => l.Quantity),
            DeliveredQuantity = lines.Sum(l => l.DeliveredQuantity),
            BalanceQuantity = lines.Sum(l => l.BalanceQuantity),
            CanModify = p.Status == EstadoProgramacion.Pendiente && !hasDeliveries,
            CreatedAt = p.CreatedAt,
            CreatedBy = p.CreatedBy,
            UpdatedAt = p.UpdatedAt,
            RowVersion = p.RowVersion,
            Detalles = lines
        };
    }

    public static async Task<ProgramacionOtros> LoadForUpdateAsync(IUnitOfWork uow, string code, CancellationToken ct)
        => await uow.Facturacion.Transacciones.ProgramacionesOtros
               .Query()
               .Include(x => x.Detalles)
               .FirstOrDefaultAsync(x => x.Code == code, ct)
           ?? throw new KeyNotFoundException($"La programación de Otros {code} no existe.");

    /// <summary>OT con líneas (tracked) para anular o cerrar sin exigir que siga programable.</summary>
    public static async Task<OrdenTrabajoVenta> LoadOrderAsync(IUnitOfWork uow, string otCode, CancellationToken ct)
        => await uow.Facturacion.Transacciones.OrdenesTrabajoVenta
               .Query()
               .Include(o => o.Detalles)
               .FirstOrDefaultAsync(o => o.Code == otCode, ct)
           ?? throw new KeyNotFoundException($"La Orden de Trabajo {otCode} no existe.");
}
