using System.Linq.Expressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common
{
    public class CancelacionVentaDetalleResponse
    {
        public string Item { get; set; } = default!;
        public string TypeCode { get; set; } = default!;
        public string PlantaCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string? DocumentBancoCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentCurrencyCode { get; set; } = default!;
        public decimal DocumentExchangeRate { get; set; }
        public decimal NetAmount { get; set; }

        /// <summary>Aplicado al documento, en la moneda del documento (las notas de crédito van en negativo).</summary>
        public decimal PaidAmount { get; set; }
        public decimal PaidSoles { get; set; }
        public decimal DifferenceSoles { get; set; }
        public string? ProvisionEntryNumber { get; set; }
        public string? ObraCode { get; set; }
        public bool IsDoubtful { get; set; }
        public decimal DoubtfulExchangeRate { get; set; }
        public bool IsDetraction { get; set; }
    }

    public class CancelacionVentaListItemResponse
    {
        public string Code { get; set; } = default!;
        public string PlantaCode { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public DateTime Date { get; set; }
        public string TipoPagoCode { get; set; } = default!;
        public string? TipoPagoName { get; set; }
        public string? PaymentNumber { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal UsedAmount { get; set; }
        public decimal DifferenceAmount { get; set; }
        public decimal AvailableAmount { get; set; }
        public string? ProvisionEntryNumber { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; } = default!;
    }

    public class CancelacionVentaResponse
    {
        public string Code { get; set; } = default!;
        public string PlantaCode { get; set; } = default!;
        public string? PlantaName { get; set; }
        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public DateTime Date { get; set; }
        public string CobradorCode { get; set; } = default!;
        public string? CobradorName { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal ExchangeRate { get; set; }
        public string TipoPagoCode { get; set; } = default!;
        public string? TipoPagoName { get; set; }
        public string? PaymentNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? BancoDocCode { get; set; }
        public string? BancoDocName { get; set; }
        public string? BancoDepCode { get; set; }
        public string? BancoDepName { get; set; }
        public bool IsDeferred { get; set; }
        public DateTime? DeferredDate { get; set; }
        public string? OperationNumber { get; set; }
        public DateTime? OperationDate { get; set; }
        public string? Description { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal UsedAmount { get; set; }
        public decimal DifferenceAmount { get; set; }
        public decimal AvailableAmount { get; set; }
        public string? ProvisionEntryNumber { get; set; }
        public DateTime? ReceptionDate { get; set; }
        public int InstallmentCount { get; set; }
        public string? InstallmentType { get; set; }
        public DateTime? FirstPaymentDate { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal Interest { get; set; }
        public string? CancellationEntryNumber { get; set; }
        public string? CanceledBy { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string? CancelReason { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        /// <summary>Se puede modificar (Pendiente y sin asiento).</summary>
        public bool CanEdit { get; set; }

        /// <summary>Se puede generar el asiento (Pendiente, sin asiento y con documentos).</summary>
        public bool CanGenerarAsiento { get; set; }

        /// <summary>Tiene asiento y se puede anular.</summary>
        public bool CanAnularAsiento { get; set; }

        /// <summary>Se puede eliminar la operación (Pendiente, sin asiento ni documentos).</summary>
        public bool CanDelete { get; set; }

        public List<CancelacionVentaDetalleResponse> Details { get; set; } = new();

        public byte[] RowVersion { get; set; } = default!;
    }

    public static class CancelacionVentaMapper
    {
        public static readonly Expression<Func<CancelacionVenta, CancelacionVentaListItemResponse>> ToListItem = c =>
            new CancelacionVentaListItemResponse
            {
                Code = c.Code,
                PlantaCode = c.PlantaCode,
                ClienteCode = c.ClienteCode,
                ClienteName = c.Cliente.Name,
                Date = c.Date,
                TipoPagoCode = c.TipoPagoCode,
                TipoPagoName = c.TipoPago.Name,
                PaymentNumber = c.PaymentNumber,
                MonedaCode = c.MonedaCode,
                ExchangeRate = c.ExchangeRate,
                TotalAmount = c.TotalAmount,
                UsedAmount = c.UsedAmount,
                DifferenceAmount = c.DifferenceAmount,
                AvailableAmount = c.AvailableAmount,
                ProvisionEntryNumber = c.ProvisionEntryNumber,
                StatusCode = (int)c.Status,
                Status = c.Status.ToString()
            };

        public static async Task<CancelacionVentaResponse?> LoadAsync(
            IUnitOfWork uow, string code, CancellationToken cancellationToken)
        {
            var c = await uow.Facturacion.Transacciones.CancelacionesVenta.Query()
                .AsNoTracking()
                .Include(x => x.Details)
                .Include(x => x.Planta)
                .Include(x => x.Cliente)
                .Include(x => x.Cobrador)
                .Include(x => x.TipoPago)
                .Include(x => x.BancoDoc)
                .Include(x => x.BancoDep)
                .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);

            return c is null ? null : Map(c);
        }

        public static CancelacionVentaResponse Map(CancelacionVenta c)
        {
            var hasAsiento = CancelacionVentaRules.HasAsiento(c);
            var hasLines = c.Details.Count > 0;

            return new CancelacionVentaResponse
            {
                Code = c.Code,
                PlantaCode = c.PlantaCode,
                PlantaName = c.Planta?.Name,
                ClienteCode = c.ClienteCode,
                ClienteName = c.Cliente?.Name,
                Date = c.Date,
                CobradorCode = c.CobradorCode,
                CobradorName = c.Cobrador?.Name,
                MonedaCode = c.MonedaCode,
                ExchangeRate = c.ExchangeRate,
                TipoPagoCode = c.TipoPagoCode,
                TipoPagoName = c.TipoPago?.Name,
                PaymentNumber = c.PaymentNumber,
                AccountNumber = c.AccountNumber,
                BancoDocCode = c.BancoDocCode,
                BancoDocName = c.BancoDoc?.Name,
                BancoDepCode = c.BancoDepCode,
                BancoDepName = c.BancoDep?.Name,
                IsDeferred = c.IsDeferred,
                DeferredDate = c.DeferredDate,
                OperationNumber = c.OperationNumber,
                OperationDate = c.OperationDate,
                Description = c.Description,
                TotalAmount = c.TotalAmount,
                UsedAmount = c.UsedAmount,
                DifferenceAmount = c.DifferenceAmount,
                AvailableAmount = c.AvailableAmount,
                ProvisionEntryNumber = c.ProvisionEntryNumber,
                ReceptionDate = c.ReceptionDate,
                InstallmentCount = c.InstallmentCount,
                InstallmentType = c.InstallmentType,
                FirstPaymentDate = c.FirstPaymentDate,
                InitialAmount = c.InitialAmount,
                Interest = c.Interest,
                CancellationEntryNumber = c.CancellationEntryNumber,
                CanceledBy = c.CanceledBy,
                CanceledAt = c.CanceledAt,
                CancelReason = c.CancelReason,
                ApprovedBy = c.ApprovedBy,
                ApprovedAt = c.ApprovedAt,
                StatusCode = (int)c.Status,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                CanEdit = CancelacionVentaRules.CanEdit(c),
                CanGenerarAsiento = c.Status == EstadoCancelacion.Pendiente && !hasAsiento && hasLines,
                CanAnularAsiento = hasAsiento && c.Status != EstadoCancelacion.Anulado,
                CanDelete = c.Status == EstadoCancelacion.Pendiente && !hasAsiento && !hasLines,
                Details = c.Details.OrderBy(d => d.Item).Select(d => new CancelacionVentaDetalleResponse
                {
                    Item = d.Item,
                    TypeCode = d.TypeCode,
                    PlantaCode = d.PlantaCode,
                    DocumentNumber = d.DocumentNumber,
                    DocumentBancoCode = d.DocumentBancoCode,
                    DocumentDate = d.DocumentDate,
                    DocumentCurrencyCode = d.DocumentCurrencyCode,
                    DocumentExchangeRate = d.DocumentExchangeRate,
                    NetAmount = d.NetAmount,
                    PaidAmount = d.PaidAmount,
                    PaidSoles = d.PaidSoles,
                    DifferenceSoles = d.DifferenceSoles,
                    ProvisionEntryNumber = d.ProvisionEntryNumber,
                    ObraCode = d.ObraCode,
                    IsDoubtful = d.IsDoubtful,
                    DoubtfulExchangeRate = d.DoubtfulExchangeRate,
                    IsDetraction = d.IsDetraction
                }).ToList(),
                RowVersion = c.RowVersion
            };
        }
    }
}
