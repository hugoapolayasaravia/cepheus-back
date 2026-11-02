using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ListadoPagosProveedor
{
    /// <summary>
    /// Legacy com_Listado_Cheques_Proveedores. Con Code se lista solo esa operación; sin Code se exige
    /// el rango de fechas de emisión y se aplican los demás filtros (todos opcionales).
    /// </summary>
    public class ListadoPagosProveedorQuery : IRequest<List<ListadoPagoProveedorRowResponse>>
    {
        public string? Ruc { get; set; }
        public string? BancoCode { get; set; }
        public string? NumeroCheque { get; set; }
        public string? NumeroComprobanteRetencion { get; set; }
        public string? Code { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }

    /// <summary>Una fila por documento cancelado (todo en soles).</summary>
    public class ListadoPagoProveedorRowResponse
    {
        public string PagoCode { get; set; } = default!;
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

        public string ProveedorCode { get; set; } = default!;
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public DateTime FechaEmisionDocumento { get; set; }
        public DateTime FechaVencimientoDocumento { get; set; }
        public decimal TotalSol { get; set; }
        public decimal PendienteSol { get; set; }
        public decimal PagoSol { get; set; }
        public string? AsientoProvisionCode { get; set; }
        public bool Detraccion { get; set; }
        public decimal BaseRetencionSol { get; set; }
        public decimal MontoRetenidoSol { get; set; }
    }
}
