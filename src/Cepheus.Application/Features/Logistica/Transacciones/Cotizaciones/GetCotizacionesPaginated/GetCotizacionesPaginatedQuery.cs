// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/GetCotizacionesPaginated/GetCotizacionesPaginatedQuery.cs
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.GetCotizacionesPaginated
{
    public class GetCotizacionesPaginatedQuery : PagedRequest, IRequest<PagedResult<CotizacionResponse>>
    {
        public string? Search { get; set; }
        public string? PlantaCode { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaLimiteDesde { get; set; }
        public DateTime? FechaLimiteHasta { get; set; }
    }
}