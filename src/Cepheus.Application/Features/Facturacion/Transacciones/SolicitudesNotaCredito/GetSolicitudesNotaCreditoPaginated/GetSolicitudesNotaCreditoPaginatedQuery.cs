using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.GetSolicitudesNotaCreditoPaginated
{
    public class GetSolicitudesNotaCreditoPaginatedQuery : PagedRequest, IRequest<PagedResult<SolicitudNotaCreditoListItemResponse>>
    {
        public string? Search { get; set; }
        public string? NegocioCode { get; set; }
        public string? ClienteCode { get; set; }
        public string? Year { get; set; }
        public string? Month { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        /// <summary>Pendiente, Facturado, Cancelado, Anulado (o su código 1..4).</summary>
        public string? Status { get; set; }

        /// <summary>Solo solicitudes Pendientes con las 5 etapas aprobadas (listas para generar la nota de crédito).</summary>
        public bool? OnlyApproved { get; set; }
    }
}
