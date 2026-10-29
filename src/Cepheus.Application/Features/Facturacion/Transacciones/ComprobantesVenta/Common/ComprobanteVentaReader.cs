using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;

public static class ComprobanteVentaReader
{
    public static async Task<ComprobanteVentaResponse> GetAsync(IUnitOfWork uow, string typeCode, string number, CancellationToken ct)
    {
        var c = await uow.Facturacion.Transacciones.ComprobantesVenta
                    .Query().AsNoTracking()
                    .Include(x => x.Planta)
                    .Include(x => x.Cliente)
                    .Include(x => x.Obra)
                    .Include(x => x.Ubigeo)
                    .Include(x => x.PaymentForm)
                    .Include(x => x.Detalles).ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(x => x.TypeCode == typeCode && x.Number == number, ct)
                ?? throw new KeyNotFoundException($"El comprobante {ComprobanteVentaRules.TypeName(typeCode)} {number} no existe.");

        var lines = c.Detalles
            .OrderBy(d => d.Line)
            .Select(d => new ComprobanteVentaDetalleResponse
            {
                Line = d.Line,
                GuiaNumber = d.GuiaNumber,
                GuiaLine = d.GuiaLine,
                OrdenTrabajoCode = d.OrdenTrabajoCode,
                OrdenTrabajoItem = d.OrdenTrabajoItem,
                ProductoTipoCode = d.ProductoTipoCode,
                ProductoCode = d.ProductoCode,
                ProductoName = d.Producto.Name,
                UnitCode = d.Producto.UnitCode,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Total = d.Total
            })
            .ToList();

        var noEntry = string.IsNullOrWhiteSpace(c.ProvisionEntry);

        return new ComprobanteVentaResponse
        {
            TypeCode = c.TypeCode,
            Number = c.Number,
            PlantCode = c.PlantCode,
            PlantName = c.Planta.Name,
            ExecutingPlantCode = c.ExecutingPlantCode,
            IssueDate = c.IssueDate,
            ClienteCode = c.ClienteCode,
            ClienteName = c.Cliente.Name,
            ClienteDocumentNumber = c.Cliente.DocumentNumber,
            ObraCode = c.ObraCode,
            ObraName = c.Obra.Description,
            VendedorCode = c.VendedorCode,
            Address = c.Address,
            UbigeoCode = c.UbigeoCode,
            UbigeoName = c.Ubigeo.FullAddress,
            PaymentFormCode = c.PaymentFormCode,
            PaymentFormName = c.PaymentForm.Name,
            CurrencyCode = c.CurrencyCode,
            ExchangeRate = c.ExchangeRate,
            OrdenTrabajoCode = c.OrdenTrabajoCode,
            Origin = c.Origin,
            DocumentFlow = c.DocumentFlow,
            IsFreeTransfer = c.IsFreeTransfer,
            Glosa = c.Glosa,
            IgvPercentage = c.IgvPercentage,
            SubtotalAmount = c.SubtotalAmount,
            IgvAmount = c.IgvAmount,
            TotalAmount = c.TotalAmount,
            TotalSoles = c.TotalSoles,
            Status = c.Status,
            IsPrinted = c.IsPrinted,
            PrintedAt = c.PrintedAt,
            ProvisionEntry = c.ProvisionEntry,
            DetractionDocument = c.DetractionDocument,
            DetractionAmount = c.DetractionAmount,
            CashDocument = c.CashDocument,
            CanceledAt = c.CanceledAt,
            CanceledBy = c.CanceledBy,
            CancelReason = c.CancelReason,
            ReplacedByNumber = c.ReplacedByNumber,
            CanModify = c.Status == EstadoComprobanteVenta.Pendiente && noEntry,
            CanAnnul = c.Status is EstadoComprobanteVenta.Pendiente or EstadoComprobanteVenta.Facturado && noEntry,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            UpdatedAt = c.UpdatedAt,
            RowVersion = c.RowVersion,
            Detalles = lines
        };
    }
}
