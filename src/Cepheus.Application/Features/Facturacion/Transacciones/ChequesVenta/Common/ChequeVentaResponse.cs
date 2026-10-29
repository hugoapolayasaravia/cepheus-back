using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common
{
    public class ChequeVentaResponse
    {
        public string Number { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public string BancoCode { get; set; } = default!;
        public string? BancoName { get; set; }
        public string PlantaCode { get; set; } = default!;
        public string? PlantaName { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? CobradorCode { get; set; }
        public string? CobradorName { get; set; }

        public string MonedaCode { get; set; } = default!;
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }

        public string? ProvisionEntryNumber { get; set; }
        public string? ReboteEntryNumber { get; set; }
        public string? CancellationEntryNumber { get; set; }
        public DateTime? CanceledAt { get; set; }

        public string? Description { get; set; }
        public string CancelacionCode { get; set; } = default!;

        public bool IsDeferred { get; set; }
        public string? OperationNumber { get; set; }
        public DateTime? OperationDate { get; set; }

        public string? MotivoReboteCode { get; set; }
        public string? MotivoReboteName { get; set; }
        public DateTime? ReboteDate { get; set; }
        public DateTime? SettledAt { get; set; }

        public int StatusCode { get; set; }
        public string Status { get; set; } = default!;

        /// <summary>Acciones que el cheque admite hoy (equivale al menú del legacy: R, N y anular operación).</summary>
        public bool CanRebotar { get; set; }
        public bool CanGenerar { get; set; }
        public bool CanAnularOperacion { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    /// <summary>Fila del listado (equivale a ad_consulta_cheques).</summary>
    public class ChequeVentaListItemResponse
    {
        public string Number { get; set; } = default!;
        public DateTime IssueDate { get; set; }
        public string CancelacionCode { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public string BancoCode { get; set; } = default!;
        public string? BancoName { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public bool IsDeferred { get; set; }
        public DateTime PaymentDate { get; set; }

        [JsonIgnore]
        public EstadoCheque StatusValue { get; set; }
        public int StatusCode => (int)StatusValue;
        public string Status => StatusValue.ToString();

        public byte[] RowVersion { get; set; } = default!;
    }

    public static class ChequeVentaMapper
    {
        public static readonly Expression<Func<ChequeVenta, ChequeVentaListItemResponse>> ToListItem =
            c => new ChequeVentaListItemResponse
            {
                Number = c.Number,
                IssueDate = c.IssueDate,
                CancelacionCode = c.CancelacionCode,
                ClienteCode = c.ClienteCode,
                ClienteName = c.Cliente.Name,
                BancoCode = c.BancoCode,
                BancoName = c.Banco.Name,
                MonedaCode = c.MonedaCode,
                TotalAmount = c.TotalAmount,
                PendingAmount = c.PendingAmount,
                IsDeferred = c.IsDeferred,
                PaymentDate = c.PaymentDate,
                StatusValue = c.Status,
                RowVersion = c.RowVersion
            };

        public static ChequeVentaResponse Map(ChequeVenta c)
            => new()
            {
                Number = c.Number,
                ClienteCode = c.ClienteCode,
                ClienteName = c.Cliente?.Name,
                BancoCode = c.BancoCode,
                BancoName = c.Banco?.Name,
                PlantaCode = c.PlantaCode,
                PlantaName = c.Planta?.Name,
                IssueDate = c.IssueDate,
                PaymentDate = c.PaymentDate,
                CobradorCode = c.CobradorCode,
                CobradorName = c.Cobrador?.Name,
                MonedaCode = c.MonedaCode,
                ExchangeRate = c.ExchangeRate,
                TotalAmount = c.TotalAmount,
                PendingAmount = c.PendingAmount,
                ProvisionEntryNumber = c.ProvisionEntryNumber,
                ReboteEntryNumber = c.ReboteEntryNumber,
                CancellationEntryNumber = c.CancellationEntryNumber,
                CanceledAt = c.CanceledAt,
                Description = c.Description,
                CancelacionCode = c.CancelacionCode,
                IsDeferred = c.IsDeferred,
                OperationNumber = c.OperationNumber,
                OperationDate = c.OperationDate,
                MotivoReboteCode = c.MotivoReboteCode,
                MotivoReboteName = c.MotivoRebote?.Name,
                ReboteDate = c.ReboteDate,
                SettledAt = c.SettledAt,
                StatusCode = (int)c.Status,
                Status = c.Status.ToString(),
                CanRebotar = ChequeVentaRules.CanRebotar(c),
                CanGenerar = ChequeVentaRules.CanGenerar(c),
                CanAnularOperacion = ChequeVentaRules.CanAnularOperacion(c),
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                RowVersion = c.RowVersion
            };

        public static async Task<ChequeVentaResponse?> LoadAsync(
            IUnitOfWork uow, string clienteCode, string bancoCode, string number, CancellationToken cancellationToken)
        {
            var cheque = await uow.Facturacion.Transacciones.ChequesVenta.Query()
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.Banco)
                .Include(c => c.Planta)
                .Include(c => c.Cobrador)
                .Include(c => c.MotivoRebote)
                .FirstOrDefaultAsync(
                    c => c.Number == number && c.ClienteCode == clienteCode && c.BancoCode == bancoCode,
                    cancellationToken);

            return cheque is null ? null : Map(cheque);
        }
    }
}
