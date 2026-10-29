using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;

public static class GuiaRemisionVentaReader
{
    public static async Task<GuiaRemisionVentaResponse> GetAsync(IUnitOfWork uow, string number, CancellationToken ct)
    {
        var g = await uow.Facturacion.Transacciones.GuiasRemisionVenta
                    .Query().AsNoTracking()
                    .Include(x => x.Planta)
                    .Include(x => x.Cliente)
                    .Include(x => x.Obra)
                    .Include(x => x.Ubigeo)
                    .Include(x => x.Transportista)
                    .Include(x => x.Chofer)
                    .Include(x => x.Vehiculo)
                    .Include(x => x.Flete)
                    .Include(x => x.Detalles).ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(x => x.Number == number, ct)
                ?? throw new KeyNotFoundException($"La guía {number} no existe.");

        var lines = g.Detalles
            .OrderBy(d => d.Line)
            .Select(d => new GuiaRemisionVentaDetalleResponse
            {
                Line = d.Line,
                OrdenTrabajoItem = d.OrdenTrabajoItem,
                ProductoTipoCode = d.ProductoTipoCode,
                ProductoCode = d.ProductoCode,
                ProductoName = d.Producto.Name,
                UnitCode = d.Producto.UnitCode,
                Quantity = d.Quantity,
                TopeQuantity = d.TopeQuantity,
                UnitPrice = d.UnitPrice,
                Total = Math.Round(d.Quantity * d.UnitPrice, 2)
            })
            .ToList();

        var pending = g.Status == EstadoGuiaRemisionVenta.Pendiente;

        return new GuiaRemisionVentaResponse
        {
            Number = g.Number,
            PlantCode = g.PlantCode,
            PlantName = g.Planta.Name,
            IssueDate = g.IssueDate,
            DispatchTime = g.DispatchTime,
            Motive = g.Motive,
            Status = g.Status,
            ClienteCode = g.ClienteCode,
            ClienteName = g.Cliente.Name,
            ObraCode = g.ObraCode,
            ObraName = g.Obra.Description,
            VendedorCode = g.VendedorCode,
            Address = g.Address,
            UbigeoCode = g.UbigeoCode,
            UbigeoName = g.Ubigeo.FullAddress,
            Description = g.Description,
            TransportistaCode = g.TransportistaCode,
            TransportistaName = g.Transportista.Name,
            ChoferCode = g.ChoferCode,
            ChoferName = g.Chofer.FullName,
            VehicleType = g.VehicleType,
            VehicleCode = g.VehicleCode,
            LicensePlate = g.Vehiculo?.LicensePlate,
            DocumentFlow = g.DocumentFlow,
            DocumentTypeCode = g.DocumentTypeCode,
            Document1Number = g.Document1Number,
            Document2Number = g.Document2Number,
            OrdenTrabajoCode = g.OrdenTrabajoCode,
            ProgramacionType = g.ProgramacionType,
            ProgramacionCode = g.ProgramacionCode,
            FleteCode = g.FleteCode,
            FleteName = g.Flete.Name,
            FleteAmount = g.FleteAmount,
            ReplacesNumber = g.ReplacesNumber,
            DevolutionMotiveCode = g.DevolutionMotiveCode,
            DevolutionActionCode = g.DevolutionActionCode,
            CurrencyCode = g.CurrencyCode,
            ExchangeRate = g.ExchangeRate,
            TicketNumber = g.TicketNumber,
            PumpCode = g.PumpCode,
            IsPrinted = g.IsPrinted,
            PrintedAt = g.PrintedAt,
            IsBalance = g.IsBalance,
            CanceledAt = g.CanceledAt,
            CanceledBy = g.CanceledBy,
            CancelReason = g.CancelReason,
            Quantity = lines.Sum(l => l.Quantity),
            CanModify = pending && !g.IsPrinted,
            CanAnnul = pending,
            CreatedAt = g.CreatedAt,
            CreatedBy = g.CreatedBy,
            UpdatedAt = g.UpdatedAt,
            RowVersion = g.RowVersion,
            Detalles = lines
        };
    }

    public static async Task<GuiaRemisionVenta> LoadForUpdateAsync(IUnitOfWork uow, string number, CancellationToken ct)
        => await uow.Facturacion.Transacciones.GuiasRemisionVenta
               .Query()
               .Include(x => x.Detalles)
               .FirstOrDefaultAsync(x => x.Number == number, ct)
           ?? throw new KeyNotFoundException($"La guía {number} no existe.");
}
