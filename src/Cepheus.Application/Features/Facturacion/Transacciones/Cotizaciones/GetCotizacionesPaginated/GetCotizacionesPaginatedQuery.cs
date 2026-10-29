using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.GetCotizacionesPaginated
{
    public class GetCotizacionesPaginatedQuery : PagedRequest, IRequest<PagedResult<CotizacionResponse>>
    {
        public string? NegocioCode { get; set; }
        public string? Year { get; set; }
        public string? VendedorCode { get; set; }
        public string? ClienteCode { get; set; }
        public string? Status { get; set; }
        public string? Search { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
