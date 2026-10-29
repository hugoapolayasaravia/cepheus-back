using System.Linq.Expressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common
{
    public class DetraccionItemResponse
    {
        public int ItemNumber { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public string ProveedorRuc { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string TipoOperacionCode { get; set; } = default!;
        public string? TipoOperacionName { get; set; }
        public string TipoBienCode { get; set; } = default!;
        public string? TipoBienName { get; set; }
        public decimal DetractionRate { get; set; }
        public decimal DepositSol { get; set; }
        public int DocumentCount { get; set; }
        public int ExportedDocumentCount { get; set; }
    }

    public class DetraccionDocumentoResponse
    {
        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public int ItemNumber { get; set; }
        public string TipoOperacionCode { get; set; } = default!;
        public string? TipoOperacionName { get; set; }
        public string TipoBienCode { get; set; } = default!;
        public string? TipoBienName { get; set; }
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public DateTime ProcessDate { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal TotalSol { get; set; }
        public decimal DetraccionSol { get; set; }
        public string? OperacionDetraccion { get; set; }
        public string? ConstanciaDetraccion { get; set; }
        public DateTime? FechaConstanciaDetraccion { get; set; }
        public bool IncludeInExport { get; set; }
    }

    public class DetraccionResponse
    {
        public string Year { get; set; } = default!;
        public string Period { get; set; } = default!;
        public string LotNumber { get; set; } = default!;
        public string Code { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ProcessDate { get; set; }
        public DateTime? DepositDate { get; set; }
        public string? PaymentReceiptNumber { get; set; }
        public DateTime? CancellationDate { get; set; }
        public decimal TotalSol { get; set; }
        public int StatusCode { get; set; }
        public string Status => ((EstadoDetraccion)StatusCode).ToString();
        public string? AsientoCode { get; set; }
        public string? ChequeCode { get; set; }
        public string? CreatedBy { get; set; }
        public List<DetraccionItemResponse> Items { get; set; } = new();

        /// <summary>Avisos del proceso de generación (por ejemplo, documentos que quedaron fuera).</summary>
        public List<string> Warnings { get; set; } = new();
    }

    public class DetraccionListItemResponse
    {
        public string Year { get; set; } = default!;
        public string Period { get; set; } = default!;
        public string LotNumber { get; set; } = default!;
        public string Code { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ProcessDate { get; set; }
        public DateTime? DepositDate { get; set; }
        public decimal TotalSol { get; set; }
        public int StatusCode { get; set; }
        public string Status => ((EstadoDetraccion)StatusCode).ToString();
    }

    public static class DetraccionMapper
    {
        public static readonly Expression<Func<Detraccion, DetraccionListItemResponse>> ToListItem = d =>
            new DetraccionListItemResponse
            {
                Year = d.Year,
                Period = d.Period,
                LotNumber = d.LotNumber,
                Code = d.Year + d.Period + d.LotNumber,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                ProcessDate = d.CreatedAt,
                DepositDate = d.DepositDate,
                TotalSol = d.TotalSol,
                StatusCode = (int)d.Status
            };

        public static readonly Expression<Func<DetraccionDocumento, DetraccionDocumentoResponse>> ToDocumento = d =>
            new DetraccionDocumentoResponse
            {
                ProveedorCode = d.ProveedorCode,
                ProveedorName = d.Item.Proveedor.LegalName,
                ItemNumber = d.ItemNumber,
                TipoOperacionCode = d.Item.TipoOperacionCode,
                TipoOperacionName = d.Item.TipoOperacion.Name,
                TipoBienCode = d.Item.TipoBienCode,
                TipoBienName = d.Item.TipoBien.Name,
                ComprobantePagoCode = d.ComprobantePagoCode,
                NumeroDocumento = d.NumeroDocumento,
                ProcessDate = d.DocumentoCompra.CreatedAt,
                FechaEmision = d.DocumentoCompra.FechaEmision,
                TotalSol = d.DocumentoCompra.TotalSol,
                DetraccionSol = d.DetraccionSol,
                OperacionDetraccion = d.DocumentoCompra.OperacionDetraccion,
                ConstanciaDetraccion = d.DocumentoCompra.ConstanciaDetraccion,
                FechaConstanciaDetraccion = d.DocumentoCompra.FechaConstanciaDetraccion,
                IncludeInExport = d.IncludeInExport
            };

        /// <summary>Las columnas char(n) de SQL Server vuelven con relleno de espacios.</summary>
        public static DetraccionDocumentoResponse Clean(DetraccionDocumentoResponse r)
        {
            r.NumeroDocumento = r.NumeroDocumento.Trim();
            r.OperacionDetraccion = Empty(r.OperacionDetraccion);
            r.ConstanciaDetraccion = Empty(r.ConstanciaDetraccion);
            return r;
        }

        private static string? Empty(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        public static async Task<DetraccionResponse?> LoadAsync(
            IUnitOfWork uow, string year, string period, string lot, CancellationToken cancellationToken)
        {
            var response = await uow.Contabilidad.Transacciones.Detracciones.Query().AsNoTracking()
                .Where(d => d.Year == year && d.Period == period && d.LotNumber == lot)
                .Select(d => new DetraccionResponse
                {
                    Year = d.Year,
                    Period = d.Period,
                    LotNumber = d.LotNumber,
                    Code = d.Year + d.Period + d.LotNumber,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    ProcessDate = d.CreatedAt,
                    DepositDate = d.DepositDate,
                    PaymentReceiptNumber = d.PaymentReceiptNumber,
                    CancellationDate = d.CancellationDate,
                    TotalSol = d.TotalSol,
                    StatusCode = (int)d.Status,
                    AsientoCode = d.AsientoCode,
                    ChequeCode = d.ChequeCode,
                    CreatedBy = d.CreatedBy,
                    Items = d.Items
                        .OrderBy(i => i.ItemNumber)
                        .Select(i => new DetraccionItemResponse
                        {
                            ItemNumber = i.ItemNumber,
                            ProveedorCode = i.ProveedorCode,
                            ProveedorName = i.Proveedor.LegalName,
                            ProveedorRuc = i.ProveedorRuc,
                            AccountNumber = i.AccountNumber,
                            TipoOperacionCode = i.TipoOperacionCode,
                            TipoOperacionName = i.TipoOperacion.Name,
                            TipoBienCode = i.TipoBienCode,
                            TipoBienName = i.TipoBien.Name,
                            DetractionRate = i.TipoBien.DetractionRate,
                            DepositSol = i.DepositSol,
                            DocumentCount = i.Documents.Count,
                            ExportedDocumentCount = i.Documents.Count(x => x.IncludeInExport)
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (response is not null)
            {
                response.PaymentReceiptNumber = Empty(response.PaymentReceiptNumber);
                response.AsientoCode = Empty(response.AsientoCode);
                response.ChequeCode = Empty(response.ChequeCode);
            }

            return response;
        }
    }
}
