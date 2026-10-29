using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GetNotasCreditoVentaPaginated
{
    /// <summary>
    /// Búsqueda de notas de crédito con parámetros opcionales (fecha desde/hasta, planta, cliente,
    /// número, estado), equivalente al SELECT de búsqueda del legacy (que usaba LIKE).
    /// </summary>
    public class GetNotasCreditoVentaPaginatedQuery : PagedRequest, IRequest<PagedResult<NotaCreditoVentaListItemResponse>>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? PlantaCode { get; set; }
        public string? ClienteCode { get; set; }

        /// <summary>Coincidencia parcial sobre el número de la nota.</summary>
        public string? Number { get; set; }

        /// <summary>Pendiente, Facturado, Cancelado, Anulado (o su código 1..4).</summary>
        public string? Status { get; set; }

        public string? Search { get; set; }
    }
}
