using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.GetCajasChicaPaginated
{
    /// <summary>
    /// Búsqueda de cajas chica (equivale al SP co_cajachica): por código, o por rango de fechas,
    /// estado y planta. Todos los filtros son opcionales.
    /// </summary>
    public class GetCajasChicaPaginatedQuery : PagedRequest, IRequest<PagedResult<CajaChicaListItemResponse>>
    {
        public string? Code { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? PlantaCode { get; set; }
        public string? TrabajadorCode { get; set; }

        /// <summary>Pendiente, Procesado, Cancelado, Anulado (o 1, 13, 3, 4).</summary>
        public string? Status { get; set; }

        /// <summary>FondoFijo, Gastos, Rendir (o F, G, R).</summary>
        public string? Type { get; set; }
    }
}
