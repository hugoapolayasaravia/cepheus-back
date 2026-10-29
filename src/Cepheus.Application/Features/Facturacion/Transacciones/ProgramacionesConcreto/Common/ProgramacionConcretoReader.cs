using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;

public static class ProgramacionConcretoReader
{
    public static async Task<ProgramacionConcretoResponse> GetAsync(IUnitOfWork uow, string code, CancellationToken ct)
    {
        var p = await uow.Facturacion.Transacciones.ProgramacionesConcreto
            .Query().AsNoTracking()
            .Include(x => x.Planta)
            .Include(x => x.Elemento)
            .Include(x => x.Detalles)
            .Include(x => x.OrdenTrabajo).ThenInclude(o => o.Cliente)
            .Include(x => x.OrdenTrabajo).ThenInclude(o => o.Obra)
            .Include(x => x.OrdenTrabajo).ThenInclude(o => o.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(x => x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La programación de Concreto {code} no existe.");

        var ot = p.OrdenTrabajo;

        var responsible = await uow.Facturacion.Catalogos.ResponsablesObra
            .Query().AsNoTracking()
            .Where(r => r.ClienteCode == ot.ClienteCode && r.ObraCode == ot.ObraCode)
            .OrderBy(r => r.Code)
            .FirstOrDefaultAsync(ct);

        var hasDeliveries = await ProgramacionConcretoRules.HasDeliveriesAsync(uow, p, ct);

        var lines = p.Detalles
            .OrderBy(d => d.Item)
            .Select(d =>
            {
                var l = ot.Detalles.First(x => x.Item == d.Item);

                return new ProgramacionConcretoDetalleResponse
                {
                    Item = d.Item,
                    ProductoTipoCode = l.ProductoTipoCode,
                    ProductoCode = l.ProductoCode,
                    ProductoName = l.Producto.Name,
                    UnitCode = l.Producto.UnitCode,
                    Quantity = d.Quantity,
                    DeliveredQuantity = d.DeliveredQuantity,
                    BalanceQuantity = d.Quantity - d.DeliveredQuantity,
                    PumpCode = d.PumpCode,
                    Support = d.Support,
                    OrderQuantity = l.Quantity,
                    ApprovedQuantity = l.ApprovedQuantity,
                    AvailableQuantity = l.ApprovedQuantity - l.ProgrammedQuantity
                };
            })
            .ToList();

        var concrete = lines.FirstOrDefault(l => l.ProductoTipoCode == ProgramacionConcretoRules.ConcretoProductoTipo);
        var quantity = concrete?.Quantity ?? lines.Sum(l => l.Quantity);
        var delivered = concrete?.DeliveredQuantity ?? lines.Sum(l => l.DeliveredQuantity);

        return new ProgramacionConcretoResponse
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
            ElementoCode = p.ElementoCode,
            ElementoName = p.Elemento.Name,
            TransportObservations = p.TransportObservations,
            RequiresLabService = p.RequiresLabService,
            Status = p.Status,
            Quantity = quantity,
            DeliveredQuantity = delivered,
            BalanceQuantity = quantity - delivered,
            CanModify = p.Status == EstadoProgramacion.Pendiente && !hasDeliveries,
            CreatedAt = p.CreatedAt,
            CreatedBy = p.CreatedBy,
            UpdatedAt = p.UpdatedAt,
            RowVersion = p.RowVersion,
            Detalles = lines
        };
    }

    /// <summary>Programación con líneas, tracked, para modificarla.</summary>
    public static async Task<ProgramacionConcreto> LoadForUpdateAsync(IUnitOfWork uow, string code, CancellationToken ct)
        => await uow.Facturacion.Transacciones.ProgramacionesConcreto
               .Query()
               .Include(x => x.Detalles)
               .FirstOrDefaultAsync(x => x.Code == code, ct)
           ?? throw new KeyNotFoundException($"La programación de Concreto {code} no existe.");
}
