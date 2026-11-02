using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common
{
    public class PagoProveedorResponse
    {
        public string Code { get; set; } = default!;
        public string NumeroCheque { get; set; } = default!;
        public string NumeroControl { get; set; } = default!;

        public string BancoCode { get; set; } = default!;
        public string BancoName { get; set; } = default!;
        public string CuentaBancariaNumero { get; set; } = default!;
        public string CuentaContableCode { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public string ProveedorRuc { get; set; } = default!;
        public string ProveedorName { get; set; } = default!;
        public string PagarA { get; set; } = default!;

        public string? NumeroComprobanteRetencion { get; set; }
        public DateTime? FechaComprobanteRetencion { get; set; }

        public DateTime FechaEmision { get; set; }
        public string MonedaCode { get; set; } = default!;

        public decimal PagoSol { get; set; }
        public decimal RetencionSol { get; set; }
        public decimal TotalSol { get; set; }
        public decimal PagoRealSol { get; set; }
        public decimal PagoSolVenta { get; set; }
        public decimal MontoReferencial { get; set; }

        public string? AsientoCancelacionCode { get; set; }
        public string? AsientoAnulacionCode { get; set; }

        public EstadoPagoProveedor Estado { get; set; }
        public TipoPagoProveedor Tipo { get; set; }
        public DateTime? FechaAnulacion { get; set; }

        public bool ComprobanteCajaBancosImpreso { get; set; }
        public bool ComprobanteRetencionImpreso { get; set; }
        public bool Afecto { get; set; }
        public bool Detraccion { get; set; }
        public bool Canje { get; set; }
        public bool DepositoElectronico { get; set; }

        public string Glosa { get; set; } = default!;
        public string PlantaCode { get; set; } = default!;
        public string? ClienteCode { get; set; }
        public string? FormaPagoCancelacionCode { get; set; }
        public int? ProveedorCuentaId { get; set; }

        public List<PagoProveedorDetalleResponse> Documentos { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public class PagoProveedorDetalleResponse
    {
        public int Id { get; set; }
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal TotalSol { get; set; }
        public decimal PendienteSol { get; set; }
        public decimal PagoSol { get; set; }
        public string? AsientoProvisionCode { get; set; }
        public bool Detraccion { get; set; }
        public decimal BaseRetencionSol { get; set; }
        public decimal MontoRetenidoSol { get; set; }
    }

    public class PagoProveedorListItemResponse
    {
        public string Code { get; set; } = default!;
        public string ProveedorRuc { get; set; } = default!;
        public string ProveedorName { get; set; } = default!;
        public DateTime FechaEmision { get; set; }
        public string NumeroCheque { get; set; } = default!;
        public string BancoCode { get; set; } = default!;
        public string BancoName { get; set; } = default!;
        public string? NumeroComprobanteRetencion { get; set; }
        public string MonedaCode { get; set; } = default!;
        public EstadoPagoProveedor Estado { get; set; }
        public bool ComprobanteCajaBancosImpreso { get; set; }
        public decimal PagoSol { get; set; }
        public decimal RetencionSol { get; set; }
        public decimal TotalSol { get; set; }
    }
}
