using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common
{
    public class SolicitudNotaCreditoDetalleResponse
    {
        public string Line { get; set; } = default!;
        public string TipoProductoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public string? ProductoName { get; set; }
        public int LineKey { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string? OrderCode { get; set; }
    }

    public class SolicitudNotaCreditoGuiaResponse
    {
        public string GuideNumber { get; set; } = default!;
        public decimal Quantity { get; set; }
    }

    public class SolicitudNotaCreditoArchivoResponse
    {
        public string FileName { get; set; } = default!;
        public string FilePath { get; set; } = default!;
    }

    public class SolicitudNotaCreditoAprobacionResponse
    {
        public int Stage { get; set; }
        public string StageName { get; set; } = default!;
        public int StatusCode { get; set; }
        public string Status { get; set; } = default!;
        public string? ActedBy { get; set; }
        public DateTime? ActedAt { get; set; }
        public string? Observation { get; set; }
    }

    public class SolicitudNotaCreditoResponse
    {
        public string Code { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;

        public string NegocioCode { get; set; } = default!;
        public string? NegocioName { get; set; }

        public DateTime IssueDate { get; set; }

        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }

        public string DocumentTypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;

        public string MotivoCode { get; set; } = default!;
        public string? MotivoName { get; set; }

        public string MonedaCode { get; set; } = default!;

        public decimal SubTotal { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string Observation { get; set; } = default!;

        public int StatusCode { get; set; }
        public string Status { get; set; } = default!;
        public bool IsEditable { get; set; }
        public bool? IsProcedent { get; set; }

        public int? RequesterUserId { get; set; }
        public bool? RequesterAccepts { get; set; }

        public int? NextStage { get; set; }
        public string? NextStageName { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;

        public List<SolicitudNotaCreditoDetalleResponse> Details { get; set; } = new();
        public List<SolicitudNotaCreditoGuiaResponse> Guides { get; set; } = new();
        public List<SolicitudNotaCreditoArchivoResponse> Files { get; set; } = new();
        public List<SolicitudNotaCreditoAprobacionResponse> Approvals { get; set; } = new();
    }

    /// <summary>Fila resumida para el listado paginado (sin líneas, guías ni archivos).</summary>
    public class SolicitudNotaCreditoListItemResponse
    {
        public string Code { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string NegocioCode { get; set; } = default!;
        public DateTime IssueDate { get; set; }
        public string ClienteCode { get; set; } = default!;
        public string? ClienteName { get; set; }
        public string DocumentTypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string MotivoCode { get; set; } = default!;
        public string? MotivoName { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal SubTotal { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal TotalAmount { get; set; }

        [JsonIgnore]
        public EstadoSolicitudNotaCredito StatusValue { get; set; }
        public int StatusCode => (int)StatusValue;
        public string Status => StatusValue.ToString();

        public bool IsEditable { get; set; }

        public int? NextStage { get; set; }
        public string? NextStageName => NextStage.HasValue
            ? ((EtapaSolicitudNotaCredito)NextStage.Value).ToString()
            : null;

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public static class SolicitudNotaCreditoMapper
    {
        public static readonly Expression<Func<SolicitudNotaCredito, SolicitudNotaCreditoListItemResponse>> ToListItem =
            s => new SolicitudNotaCreditoListItemResponse
            {
                Code = s.Code,
                Year = s.Year,
                Month = s.Month,
                NegocioCode = s.NegocioCode,
                IssueDate = s.IssueDate,
                ClienteCode = s.ClienteCode,
                ClienteName = s.Cliente.Name,
                DocumentTypeCode = s.DocumentTypeCode,
                DocumentNumber = s.DocumentNumber,
                MotivoCode = s.MotivoCode,
                MotivoName = s.Motivo.Name,
                MonedaCode = s.MonedaCode,
                SubTotal = s.SubTotal,
                IgvAmount = s.IgvAmount,
                TotalAmount = s.TotalAmount,
                StatusValue = s.Status,
                IsEditable = s.IsEditable,
                NextStage = s.Approvals
                    .Where(a => a.Status == EstadoAprobacionSolicitud.Pendiente)
                    .OrderBy(a => a.Stage)
                    .Select(a => (int?)a.Stage)
                    .FirstOrDefault(),
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                RowVersion = s.RowVersion
            };

        /// <summary>Requiere Negocio, Cliente, Motivo, Details(+Producto), Guides, Files y Approvals cargados.</summary>
        public static SolicitudNotaCreditoResponse Map(SolicitudNotaCredito s)
        {
            var nextStage = s.Status == EstadoSolicitudNotaCredito.Pendiente
                ? s.Approvals
                    .Where(a => a.Status == EstadoAprobacionSolicitud.Pendiente)
                    .OrderBy(a => a.Stage)
                    .Select(a => (EtapaSolicitudNotaCredito?)a.Stage)
                    .FirstOrDefault()
                : null;

            return new SolicitudNotaCreditoResponse
            {
                Code = s.Code,
                Year = s.Year,
                Month = s.Month,
                NegocioCode = s.NegocioCode,
                NegocioName = s.Negocio?.Name,
                IssueDate = s.IssueDate,
                ClienteCode = s.ClienteCode,
                ClienteName = s.Cliente?.Name,
                DocumentTypeCode = s.DocumentTypeCode,
                DocumentNumber = s.DocumentNumber,
                MotivoCode = s.MotivoCode,
                MotivoName = s.Motivo?.Name,
                MonedaCode = s.MonedaCode,
                SubTotal = s.SubTotal,
                IgvAmount = s.IgvAmount,
                TotalAmount = s.TotalAmount,
                Observation = s.Observation,
                StatusCode = (int)s.Status,
                Status = s.Status.ToString(),
                IsEditable = s.IsEditable,
                IsProcedent = s.IsProcedent,
                RequesterUserId = s.RequesterUserId,
                RequesterAccepts = s.RequesterAccepts,
                NextStage = nextStage.HasValue ? (int)nextStage.Value : null,
                NextStageName = nextStage?.ToString(),
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                UpdatedAt = s.UpdatedAt,
                RowVersion = s.RowVersion,
                Details = s.Details
                    .OrderBy(d => d.Line)
                    .Select(d => new SolicitudNotaCreditoDetalleResponse
                    {
                        Line = d.Line,
                        TipoProductoCode = d.TipoProductoCode,
                        ProductoCode = d.ProductoCode,
                        ProductoName = d.Producto?.Name,
                        LineKey = d.LineKey,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Total = d.Total,
                        OrderCode = d.OrderCode
                    }).ToList(),
                Guides = s.Guides
                    .OrderBy(g => g.GuideNumber)
                    .Select(g => new SolicitudNotaCreditoGuiaResponse
                    {
                        GuideNumber = g.GuideNumber,
                        Quantity = g.Quantity
                    }).ToList(),
                Files = s.Files
                    .OrderBy(f => f.FileName)
                    .Select(f => new SolicitudNotaCreditoArchivoResponse
                    {
                        FileName = f.FileName,
                        FilePath = f.FilePath
                    }).ToList(),
                Approvals = s.Approvals
                    .OrderBy(a => a.Stage)
                    .Select(a => new SolicitudNotaCreditoAprobacionResponse
                    {
                        Stage = (int)a.Stage,
                        StageName = a.Stage.ToString(),
                        StatusCode = (int)a.Status,
                        Status = a.Status.ToString(),
                        ActedBy = a.ActedBy,
                        ActedAt = a.ActedAt,
                        Observation = a.Observation
                    }).ToList()
            };
        }

        public static async Task<SolicitudNotaCreditoResponse?> LoadAsync(
            IUnitOfWork uow, string year, string month, string code, CancellationToken cancellationToken)
        {
            var solicitud = await uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query()
                .AsNoTracking()
                .Include(x => x.Negocio)
                .Include(x => x.Cliente)
                .Include(x => x.Motivo)
                .Include(x => x.Details).ThenInclude(d => d.Producto)
                .Include(x => x.Guides)
                .Include(x => x.Files)
                .Include(x => x.Approvals)
                .FirstOrDefaultAsync(x => x.Code == code && x.Year == year && x.Month == month, cancellationToken);

            return solicitud is null ? null : Map(solicitud);
        }
    }
}
