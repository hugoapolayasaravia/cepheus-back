using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetDetraccionesPaginated
{
    /// <summary>
    /// Búsqueda de lotes de detracción (equivale al SP COM_SP_BUS_DETRACCION_COM). Todos los filtros
    /// son opcionales.
    /// </summary>
    public class GetDetraccionesPaginatedQuery : PagedRequest, IRequest<PagedResult<DetraccionListItemResponse>>
    {
        public string? Year { get; set; }
        public string? Period { get; set; }
        public string? LotNumber { get; set; }

        /// <summary>Fecha de proceso (día en que se generó el lote).</summary>
        public DateTime? ProcessDate { get; set; }

        /// <summary>Pendiente, Emitido, Cancelado (o 1, 19, 3).</summary>
        public string? Status { get; set; }
    }
}
