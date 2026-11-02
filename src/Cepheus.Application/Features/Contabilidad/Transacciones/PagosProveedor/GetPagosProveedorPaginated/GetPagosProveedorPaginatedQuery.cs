using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GetPagosProveedorPaginated
{
    /// <summary>Parámetros de com_con_cheques: RUC, banco, cheque, comprobante de retención, operación y rango de fechas.</summary>
    public class GetPagosProveedorPaginatedQuery : PagedRequest, IRequest<PagedResult<PagoProveedorListItemResponse>>
    {
        public string? Ruc { get; set; }
        public string? BancoCode { get; set; }
        public string? NumeroCheque { get; set; }
        public string? NumeroComprobanteRetencion { get; set; }
        public string? Code { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}
