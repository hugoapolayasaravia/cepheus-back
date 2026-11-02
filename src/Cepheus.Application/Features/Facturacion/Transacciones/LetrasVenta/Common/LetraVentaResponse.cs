using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common
{
    /// <summary>Documento que la letra canjea (línea de la cancelación que la originó).</summary>
    public class LetraVentaDocumentoResponse
    {
        public string Item { get; set; } = default!;
        public string TypeCode { get; set; } = default!;
        public string PlantaCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public DateTime DocumentDate { get; set; }
        public string DocumentCurrencyCode { get; set; } = default!;
        public decimal DocumentExchangeRate { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PaidSoles { get; set; }
        public decimal DifferenceSoles { get; set; }
        public string? ObraCode { get; set; }
        public bool IsDetraction { get; set; }
    }

    public class LetraVentaResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string? PlantaName { get; set; }
        public string Number { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public string? BancoCode { get; set; }
        public string? BancoName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string CobradorCode { get; set; } = default!;
        public string? CobradorName { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal Interest { get; set; }
        public string? ProvisionEntryNumber { get; set; }
        public string? CancellationEntryNumber { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? SettledAt { get; set; }
        public string? Description { get; set; }
        public string CancelacionCode { get; set; } = default!;
        public bool IsPrinted { get; set; }
        public bool AcceptedByClient { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; } = default!;

        /// <summary>Se pueden corregir fechas, aceptación y glosa (solo letras por cobrar).</summary>
        public bool CanEdit { get; set; }
        public bool CanProtestar { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;

        public List<LetraVentaDocumentoResponse> Documents { get; set; } = new();
    }

    /// <summary>Fila del listado (equivale a ad_consulta_letras).</summary>
    public class LetraVentaListItemResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Number { get; set; } = default!;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string CancelacionCode { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public string? BancoCode { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public bool AcceptedByClient { get; set; }

        [JsonIgnore]
        public EstadoLetra StatusValue { get; set; }
        public int StatusCode => (int)StatusValue;
        public string Status => StatusValue.ToString();

        public byte[] RowVersion { get; set; } = default!;
    }

    public static class LetraVentaMapper
    {
        public static readonly Expression<Func<LetraVenta, LetraVentaListItemResponse>> ToListItem =
            l => new LetraVentaListItemResponse
            {
                PlantaCode = l.PlantaCode,
                Number = l.Number,
                IssueDate = l.IssueDate,
                DueDate = l.DueDate,
                CancelacionCode = l.CancelacionCode,
                ClienteCode = l.ClienteCode,
                ClienteName = l.Cliente.Name,
                BancoCode = l.BancoCode,
                MonedaCode = l.MonedaCode,
                TotalAmount = l.TotalAmount,
                PendingAmount = l.PendingAmount,
                AcceptedByClient = l.AcceptedByClient,
                StatusValue = l.Status,
                RowVersion = l.RowVersion
            };

        public static LetraVentaResponse Map(LetraVenta l, IEnumerable<CancelacionVentaDetalle> lines)
            => new()
            {
                PlantaCode = l.PlantaCode,
                PlantaName = l.Planta?.Name,
                Number = l.Number,
                ClienteCode = l.ClienteCode,
                ClienteName = l.Cliente?.Name,
                BancoCode = l.BancoCode,
                BancoName = l.Banco?.Name,
                IssueDate = l.IssueDate,
                DueDate = l.DueDate,
                CobradorCode = l.CobradorCode,
                CobradorName = l.Cobrador?.Name,
                MonedaCode = l.MonedaCode,
                ExchangeRate = l.ExchangeRate,
                TotalAmount = l.TotalAmount,
                PendingAmount = l.PendingAmount,
                Interest = l.Interest,
                ProvisionEntryNumber = l.ProvisionEntryNumber,
                CancellationEntryNumber = l.CancellationEntryNumber,
                CanceledAt = l.CanceledAt,
                SettledAt = l.SettledAt,
                Description = l.Description,
                CancelacionCode = l.CancelacionCode,
                IsPrinted = l.IsPrinted,
                AcceptedByClient = l.AcceptedByClient,
                StatusCode = (int)l.Status,
                Status = l.Status.ToString(),
                CanEdit = LetraVentaRules.CanEdit(l),
                CanProtestar = LetraVentaRules.CanProtestar(l),
                CreatedAt = l.CreatedAt,
                CreatedBy = l.CreatedBy,
                UpdatedAt = l.UpdatedAt,
                RowVersion = l.RowVersion,
                Documents = lines.OrderBy(d => d.Item).Select(d => new LetraVentaDocumentoResponse
                {
                    Item = d.Item,
                    TypeCode = d.TypeCode,
                    PlantaCode = d.PlantaCode,
                    DocumentNumber = d.DocumentNumber,
                    DocumentDate = d.DocumentDate,
                    DocumentCurrencyCode = d.DocumentCurrencyCode,
                    DocumentExchangeRate = d.DocumentExchangeRate,
                    NetAmount = d.NetAmount,
                    PaidAmount = d.PaidAmount,
                    PaidSoles = d.PaidSoles,
                    DifferenceSoles = d.DifferenceSoles,
                    ObraCode = d.ObraCode,
                    IsDetraction = d.IsDetraction
                }).ToList()
            };

        public static async Task<LetraVentaResponse?> LoadAsync(
            IUnitOfWork uow, string plantaCode, string number, CancellationToken cancellationToken)
        {
            var letra = await uow.Facturacion.Transacciones.LetrasVenta.Query()
                .AsNoTracking()
                .Include(l => l.Planta)
                .Include(l => l.Cliente)
                .Include(l => l.Banco)
                .Include(l => l.Cobrador)
                .FirstOrDefaultAsync(l => l.PlantaCode == plantaCode && l.Number == number, cancellationToken);

            if (letra is null)
            {
                return null;
            }

            var lines = await uow.Facturacion.Transacciones.CancelacionesVentaDetalle.Query()
                .AsNoTracking()
                .Where(d => d.CancelacionCode == letra.CancelacionCode)
                .ToListAsync(cancellationToken);

            return Map(letra, lines);
        }
    }
}
