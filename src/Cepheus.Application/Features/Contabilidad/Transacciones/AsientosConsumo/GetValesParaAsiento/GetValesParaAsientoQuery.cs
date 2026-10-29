using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.GetValesParaAsiento
{
    /// <summary>
    /// Búsqueda de líneas de vale de salida para el asiento de consumo (legacy: SP CO_consultaconsumo
    /// @fei, @fef, @flg, @pla). Trae las líneas de vale Procesadas de la planta cuya fecha de entrega está en el
    /// rango. SoloPendientes = true (flag 'S' del legacy, valor por defecto) limita a las que aún no tienen asiento.
    /// </summary>
    public class GetValesParaAsientoQuery : PagedRequest, IRequest<PagedResult<ValeConsumoAsientoItemResponse>>
    {
        public string? PlantaCode { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool? SoloPendientes { get; set; }
    }
}
