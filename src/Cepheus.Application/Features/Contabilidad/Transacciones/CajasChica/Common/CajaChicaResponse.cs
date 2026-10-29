using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common
{
    public class CajaChicaDetalleResponse
    {
        public int ItemNumber { get; set; }
        public int OrigenCode { get; set; }
        public string Origen { get; set; } = default!;
        public string? ReferenceCode { get; set; }
        public string PlantaCode { get; set; } = default!;
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public DateTime FechaEmision { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal TipoCambio { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnidad { get; set; }
        public decimal Parcial { get; set; }
        public decimal Igv { get; set; }
        public decimal NoGravable { get; set; }
        public decimal Renta { get; set; }
        public decimal Fonavi { get; set; }
        public decimal Servicio { get; set; }
        public decimal IgvExterior { get; set; }
        public decimal Total { get; set; }
        public string CuentaContableCode { get; set; } = default!;
        public string? AnexoTipoCode { get; set; }
        public string? AnexoCode { get; set; }
        public string? Concepto { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public string? ProveedorRuc { get; set; }
        public string? DocumentoReferenciaComprobantePagoCode { get; set; }
        public string? DocumentoReferenciaNumero { get; set; }
        public DateTime? DocumentoReferenciaFecha { get; set; }
    }

    public class CajaChicaResponse
    {
        public string Code { get; set; } = default!;
        public string PlantaCode { get; set; } = default!;
        public string? PlantaName { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string? TrabajadorName { get; set; }
        public DateTime IssueDate { get; set; }
        public string? Observations { get; set; }
        public decimal MontoSol { get; set; }
        public decimal TotalSol { get; set; }
        public string? ProvisionAsientoCode { get; set; }
        public string? ReversionAsientoCode { get; set; }
        public int TypeCode { get; set; }
        public string Type { get; set; } = default!;
        public bool IsPrinted { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
        public List<CajaChicaDetalleResponse> Details { get; set; } = new();
    }

    /// <summary>Fila del listado (equivale al SP co_cajachica del legacy).</summary>
    public class CajaChicaListItemResponse
    {
        public string Code { get; set; } = default!;
        public string PlantaCode { get; set; } = default!;
        public string TrabajadorCode { get; set; } = default!;
        public string? TrabajadorName { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal MontoSol { get; set; }
        public decimal TotalSol { get; set; }
        public string? ProvisionAsientoCode { get; set; }
        public string? ReversionAsientoCode { get; set; }
        public bool IsPrinted { get; set; }

        [JsonIgnore]
        public TipoCajaChica TypeValue { get; set; }
        public int TypeCode => (int)TypeValue;
        public string Type => TypeValue.ToString();

        [JsonIgnore]
        public EstadoCajaChica StatusValue { get; set; }
        public int StatusCode => (int)StatusValue;
        public string Status => StatusValue.ToString();

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public static class CajaChicaMapper
    {
        public static string BuildName(string? paternal, string? maternal, string? names)
            => string.Join(" ", new[] { paternal, maternal, names }.Where(s => !string.IsNullOrWhiteSpace(s)));

        public static readonly Expression<Func<CajaChica, CajaChicaListItemResponse>> ToListItem =
            c => new CajaChicaListItemResponse
            {
                Code = c.Code,
                PlantaCode = c.PlantaCode,
                TrabajadorCode = c.TrabajadorCode,
                TrabajadorName = ((c.Trabajador.PaternalSurname ?? "") + " " + (c.Trabajador.MaternalSurname ?? "") + " " + (c.Trabajador.FirstNames ?? "")).Trim(),
                IssueDate = c.IssueDate,
                MontoSol = c.MontoSol,
                TotalSol = c.TotalSol,
                ProvisionAsientoCode = c.ProvisionAsientoCode,
                ReversionAsientoCode = c.ReversionAsientoCode,
                IsPrinted = c.IsPrinted,
                TypeValue = c.Type,
                StatusValue = c.Status,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                RowVersion = c.RowVersion
            };

        /// <summary>Requiere Planta, Trabajador y Details(+Proveedor) cargados.</summary>
        public static CajaChicaResponse Map(CajaChica c)
            => new()
            {
                Code = c.Code,
                PlantaCode = c.PlantaCode,
                PlantaName = c.Planta?.Name,
                TrabajadorCode = c.TrabajadorCode,
                TrabajadorName = c.Trabajador is null
                    ? null
                    : BuildName(c.Trabajador.PaternalSurname, c.Trabajador.MaternalSurname, c.Trabajador.FirstNames),
                IssueDate = c.IssueDate,
                Observations = c.Observations,
                MontoSol = c.MontoSol,
                TotalSol = c.TotalSol,
                ProvisionAsientoCode = c.ProvisionAsientoCode,
                ReversionAsientoCode = c.ReversionAsientoCode,
                TypeCode = (int)c.Type,
                Type = c.Type.ToString(),
                IsPrinted = c.IsPrinted,
                StatusCode = (int)c.Status,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                RowVersion = c.RowVersion,
                Details = c.Details
                    .OrderBy(d => d.ItemNumber)
                    .Select(d => new CajaChicaDetalleResponse
                    {
                        ItemNumber = d.ItemNumber,
                        OrigenCode = (int)d.Origen,
                        Origen = d.Origen.ToString(),
                        ReferenceCode = d.ReferenceCode,
                        PlantaCode = d.PlantaCode,
                        ComprobantePagoCode = d.ComprobantePagoCode,
                        NumeroDocumento = d.NumeroDocumento,
                        FechaEmision = d.FechaEmision,
                        MonedaCode = d.MonedaCode,
                        TipoCambio = d.TipoCambio,
                        Cantidad = d.Cantidad,
                        PrecioUnidad = d.PrecioUnidad,
                        Parcial = d.Parcial,
                        Igv = d.Igv,
                        NoGravable = d.NoGravable,
                        Renta = d.Renta,
                        Fonavi = d.Fonavi,
                        Servicio = d.Servicio,
                        IgvExterior = d.IgvExterior,
                        Total = d.Total,
                        CuentaContableCode = d.CuentaContableCode,
                        AnexoTipoCode = d.AnexoTipoCode,
                        AnexoCode = d.AnexoCode,
                        Concepto = d.Concepto,
                        ProveedorCode = d.ProveedorCode,
                        ProveedorName = d.Proveedor?.LegalName,
                        ProveedorRuc = d.Proveedor?.DocumentNumber,
                        DocumentoReferenciaComprobantePagoCode = d.DocumentoReferenciaComprobantePagoCode,
                        DocumentoReferenciaNumero = d.DocumentoReferenciaNumero,
                        DocumentoReferenciaFecha = d.DocumentoReferenciaFecha
                    }).ToList()
            };

        public static async Task<CajaChicaResponse?> LoadAsync(
            IUnitOfWork uow, string code, CancellationToken cancellationToken)
        {
            var caja = await uow.Contabilidad.Transacciones.CajasChica.Query()
                .AsNoTracking()
                .Include(c => c.Planta)
                .Include(c => c.Trabajador)
                .Include(c => c.Details).ThenInclude(d => d.Proveedor)
                .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

            return caja is null ? null : Map(caja);
        }
    }
}
