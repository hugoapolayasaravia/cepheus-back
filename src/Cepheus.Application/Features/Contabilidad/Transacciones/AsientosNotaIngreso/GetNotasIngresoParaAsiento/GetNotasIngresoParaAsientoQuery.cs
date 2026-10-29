using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GetNotasIngresoParaAsiento
{
    /// <summary>
    /// Búsqueda de notas de ingreso para generar su asiento (legacy: SP CO_Ingresos @fei, @fef, @flg, @pla).
    /// Trae las notas de ingreso Procesadas de la planta cuya fecha de recepción (Fecha_ent) está en el rango.
    /// SoloPendientes = true (flag 'S' del legacy, valor por defecto) limita a las que aún no tienen asiento.
    /// </summary>
    public class GetNotasIngresoParaAsientoQuery
        : PagedRequest, IRequest<PagedResult<NotaIngresoAsientoItemResponse>>
    {
        public string? PlantaCode { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool? SoloPendientes { get; set; }
    }
}
