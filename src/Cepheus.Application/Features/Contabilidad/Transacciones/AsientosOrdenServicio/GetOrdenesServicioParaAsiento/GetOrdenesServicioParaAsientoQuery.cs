using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.GetOrdenesServicioParaAsiento
{
    /// <summary>
    /// Búsqueda de órdenes de servicio para generar su asiento (legacy: SP CO_OrdenServicio @fei, @fef, @flg, @pla).
    /// Trae las órdenes Procesadas de la planta cuya fecha de recepción (Fecha_ent) está en el rango.
    /// SoloPendientes = true (flag 'S' del legacy, valor por defecto) limita a las que aún no tienen asiento.
    /// </summary>
    public class GetOrdenesServicioParaAsientoQuery
        : PagedRequest, IRequest<PagedResult<OrdenServicioAsientoItemResponse>>
    {
        public string? PlantaCode { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool? SoloPendientes { get; set; }
    }
}
