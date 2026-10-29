using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common
{
    public class NotaCreditoVentaDetalleResponse
    {
        public string Line { get; set; } = default!;
        public string? GuideNumber { get; set; }
        public string TipoProductoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public string? ProductoName { get; set; }
        public string Description { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal DispatchedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public int LineKey { get; set; }
        public string? OrderCode { get; set; }
    }

    public class NotaCreditoVentaResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string? PlantaName { get; set; }
        public string Number { get; set; } = default!;
        public DateTime IssueDate { get; set; }

        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public string ObraCode { get; set; } = default!;
        public string? ObraDescription { get; set; }
        public string? VendedorCode { get; set; }

        public string DeliveryAddress { get; set; } = default!;
        public string DeliveryUbigeoCode { get; set; } = default!;

        public bool IsPrinted { get; set; }
        public DateTime? PrintedAt { get; set; }

        public string FormaPagoVentaCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public decimal ExchangeRate { get; set; }
        public string? OrderCode { get; set; }
        public string Description { get; set; } = default!;
        public string ExecutingPlantaCode { get; set; } = default!;

        public decimal SubTotal { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }

        public string? ProvisionEntryNumber { get; set; }
        public string? CancellationEntryNumber { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string? CancelReason { get; set; }

        public string Type { get; set; } = default!;
        public string DocumentTypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string MotivoDevolucionCode { get; set; } = default!;
        public string? MotivoDevolucionName { get; set; }

        public string? SolicitudCode { get; set; }
        public string? SolicitudYear { get; set; }
        public string? SolicitudMonth { get; set; }

        public string? CashDocument { get; set; }

        public int StatusCode { get; set; }
        public string Status { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;

        public List<NotaCreditoVentaDetalleResponse> Details { get; set; } = new();
    }

    /// <summary>Fila del listado paginado (equivale a la búsqueda del legacy).</summary>
    public class NotaCreditoVentaListItemResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Number { get; set; } = default!;
        public DateTime IssueDate { get; set; }
        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public string ObraCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public decimal SubTotal { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public string DocumentTypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string? SolicitudCode { get; set; }
        public string? SolicitudYear { get; set; }
        public string? SolicitudMonth { get; set; }
        public bool IsPrinted { get; set; }

        [JsonIgnore]
        public EstadoNotaCredito StatusValue { get; set; }
        public int StatusCode => (int)StatusValue;
        public string Status => StatusValue.ToString();

        public DateTime CreatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public static class NotaCreditoVentaMapper
    {
        public static readonly Expression<Func<NotaCreditoVenta, NotaCreditoVentaListItemResponse>> ToListItem =
            n => new NotaCreditoVentaListItemResponse
            {
                PlantaCode = n.PlantaCode,
                Number = n.Number,
                IssueDate = n.IssueDate,
                ClienteCode = n.ClienteCode,
                ClienteName = n.Cliente.Name,
                ObraCode = n.ObraCode,
                MonedaCode = n.MonedaCode,
                SubTotal = n.SubTotal,
                IgvAmount = n.IgvAmount,
                TotalAmount = n.TotalAmount,
                PendingAmount = n.PendingAmount,
                DocumentTypeCode = n.DocumentTypeCode,
                DocumentNumber = n.DocumentNumber,
                SolicitudCode = n.SolicitudCode,
                SolicitudYear = n.SolicitudYear,
                SolicitudMonth = n.SolicitudMonth,
                IsPrinted = n.IsPrinted,
                StatusValue = n.Status,
                CreatedAt = n.CreatedAt,
                RowVersion = n.RowVersion
            };

        /// <summary>Requiere Planta, Cliente, Obra, MotivoDevolucion y Details(+Producto) cargados.</summary>
        public static NotaCreditoVentaResponse Map(NotaCreditoVenta n)
            => new()
            {
                PlantaCode = n.PlantaCode,
                PlantaName = n.Planta?.Name,
                Number = n.Number,
                IssueDate = n.IssueDate,
                ClienteCode = n.ClienteCode,
                ClienteName = n.Cliente?.Name,
                ObraCode = n.ObraCode,
                ObraDescription = n.Obra?.Description,
                VendedorCode = n.VendedorCode,
                DeliveryAddress = n.DeliveryAddress,
                DeliveryUbigeoCode = n.DeliveryUbigeoCode,
                IsPrinted = n.IsPrinted,
                PrintedAt = n.PrintedAt,
                FormaPagoVentaCode = n.FormaPagoVentaCode,
                MonedaCode = n.MonedaCode,
                ExchangeRate = n.ExchangeRate,
                OrderCode = n.OrderCode,
                Description = n.Description,
                ExecutingPlantaCode = n.ExecutingPlantaCode,
                SubTotal = n.SubTotal,
                IgvAmount = n.IgvAmount,
                TotalAmount = n.TotalAmount,
                PendingAmount = n.PendingAmount,
                ProvisionEntryNumber = n.ProvisionEntryNumber,
                CancellationEntryNumber = n.CancellationEntryNumber,
                CanceledAt = n.CanceledAt,
                CancelReason = n.CancelReason,
                Type = n.Type,
                DocumentTypeCode = n.DocumentTypeCode,
                DocumentNumber = n.DocumentNumber,
                MotivoDevolucionCode = n.MotivoDevolucionCode,
                MotivoDevolucionName = n.MotivoDevolucion?.Name,
                SolicitudCode = n.SolicitudCode,
                SolicitudYear = n.SolicitudYear,
                SolicitudMonth = n.SolicitudMonth,
                CashDocument = n.CashDocument,
                StatusCode = (int)n.Status,
                Status = n.Status.ToString(),
                CreatedAt = n.CreatedAt,
                CreatedBy = n.CreatedBy,
                UpdatedAt = n.UpdatedAt,
                RowVersion = n.RowVersion,
                Details = n.Details
                    .OrderBy(d => d.Line)
                    .Select(d => new NotaCreditoVentaDetalleResponse
                    {
                        Line = d.Line,
                        GuideNumber = d.GuideNumber,
                        TipoProductoCode = d.TipoProductoCode,
                        ProductoCode = d.ProductoCode,
                        ProductoName = d.Producto?.Name,
                        Description = d.Description,
                        Quantity = d.Quantity,
                        DispatchedQuantity = d.DispatchedQuantity,
                        UnitPrice = d.UnitPrice,
                        Total = d.Total,
                        LineKey = d.LineKey,
                        OrderCode = d.OrderCode
                    }).ToList()
            };

        public static async Task<NotaCreditoVentaResponse?> LoadAsync(
            IUnitOfWork uow, string plantaCode, string number, CancellationToken cancellationToken)
        {
            var nota = await uow.Facturacion.Transacciones.NotasCreditoVenta.Query()
                .AsNoTracking()
                .Include(n => n.Planta)
                .Include(n => n.Cliente)
                .Include(n => n.Obra)
                .Include(n => n.MotivoDevolucion)
                .Include(n => n.Details).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(n => n.PlantaCode == plantaCode && n.Number == number, cancellationToken);

            return nota is null ? null : Map(nota);
        }
    }
}
