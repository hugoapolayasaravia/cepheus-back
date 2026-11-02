using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ComprobanteCajaBancosPagoProveedor
{
    /// <summary>Legacy ad_rpt_cajabancos: datos para imprimir el comprobante caja-bancos (asiento de cancelación).</summary>
    public record ComprobanteCajaBancosPagoProveedorQuery(string Code) : IRequest<ComprobanteCajaBancosPagoProveedorResponse>;

    public class ComprobanteCajaBancosPagoProveedorResponse
    {
        public string PagoCode { get; set; } = default!;
        public string AsientoCancelacionCode { get; set; } = default!;
        public string? NumeroComprobanteRetencion { get; set; }

        /// <summary>Número de cheque y número de control, como en el legacy (cheque-control).</summary>
        public string NumeroCheque { get; set; } = default!;

        public DateTime FechaEmision { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public string ProveedorName { get; set; } = default!;
        public string PagarA { get; set; } = default!;
        public string BancoCode { get; set; } = default!;
        public string BancoName { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public decimal TotalSol { get; set; }
        public decimal RetencionSol { get; set; }
        public string Glosa { get; set; } = default!;

        public string AsientoAnio { get; set; } = default!;
        public string AsientoMes { get; set; } = default!;
        public string TipoAsientoCode { get; set; } = default!;
        public string AsientoNumero { get; set; } = default!;
        public DateTime FechaProceso { get; set; }
        public decimal TipoCambio { get; set; }
        public string AsientoDescripcion { get; set; } = default!;
        public EstadoAsiento AsientoEstado { get; set; }
        public decimal TotalCargosSol { get; set; }
        public decimal TotalAbonosSol { get; set; }

        public List<CajaBancosLineaResponse> Lineas { get; set; } = new();
    }

    public class CajaBancosLineaResponse
    {
        public int ItemNumber { get; set; }
        public string PlantaCode { get; set; } = default!;
        public string CuentaContableCode { get; set; } = default!;
        public string? AnexoTipoCode { get; set; }
        public string? AnexoCode { get; set; }
        public string Descripcion { get; set; } = default!;
        public string? NumeroDocumento { get; set; }
        public decimal CargoSol { get; set; }
        public decimal AbonoSol { get; set; }
        public bool EsDestino { get; set; }
        public TipoContabilidad TipoContabilidad { get; set; }
    }
}
