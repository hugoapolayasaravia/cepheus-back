// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/GetOrdenesCompraAdministracionPaginated/GetOrdenesCompraAdministracionPaginatedQuery.cs
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.GetOrdenesCompraAdmPaginated
{
    /// <summary>
    /// Filtros calcados de Logi_sp_Listado_MOCompras_Pedientes / _Aprobacion (el valor 'T' = todos
    /// del legacy pasa a ser "filtro no enviado"): si viene Code busca exacto; si no, filtra por rango
    /// de fecha de proceso. Más Proveedor / Estado / Usuario (CreatedBy) / FormaPago.
    /// Para la bandeja de aprobación enviar Estado=Pendiente.
    /// </summary>
    public class GetOrdenesCompraAdmPaginatedQuery
        : PagedRequest, IRequest<PagedResult<OrdenCompraAdmListItemResponse>>
    {
        public string? Code { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? ProveedorCode { get; set; }
        public string? Estado { get; set; }
        public string? Usuario { get; set; }
        public string? FormaPagoCode { get; set; }
    }
}
