using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.GetAsientosContablesPaginated
{
    /// <summary>
    /// Búsqueda de asientos. El script legacy no trae un SP de búsqueda de asientos: los filtros son una
    /// propuesta (período, tipo, estado, origen, fechas, documento y texto de la descripción).
    /// </summary>
    public class GetAsientosContablesPaginatedQuery
        : PagedRequest, IRequest<PagedResult<AsientoContableListItemResponse>>
    {
        public string? Anio { get; set; }
        public string? Mes { get; set; }
        public string? TipoAsientoCode { get; set; }
        public EstadoAsiento? Estado { get; set; }
        public OrigenAsiento? Origen { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        /// <summary>Número de documento de alguna línea del asiento (exacto).</summary>
        public string? NumeroDocumento { get; set; }
        /// <summary>Texto contenido en la descripción.</summary>
        public string? Search { get; set; }
    }
}
