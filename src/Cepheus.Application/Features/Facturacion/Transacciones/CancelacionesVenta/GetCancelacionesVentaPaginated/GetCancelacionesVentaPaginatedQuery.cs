using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GetCancelacionesVentaPaginated
{
    /// <summary>
    /// Listado de cancelaciones (legacy dw_fac_cab_cancelaciones: cliente + rango de fechas de la cancelación).
    /// Con <c>Code</c> solo se busca por él.
    /// </summary>
    public class GetCancelacionesVentaPaginatedQuery : PagedRequest, IRequest<PagedResult<CancelacionVentaListItemResponse>>
    {
        public string? Code { get; set; }
        public string? ClienteCode { get; set; }
        public string? PlantaCode { get; set; }
        public string? TipoPagoCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        /// <summary>Pendiente, Facturado, Cancelado, Anulado (o 1..4).</summary>
        public string? Status { get; set; }
    }
}
