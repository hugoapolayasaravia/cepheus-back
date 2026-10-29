// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/GetOrdenesCompraPaginated/GetOrdenesCompraPaginatedQuery.cs
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.GetOrdenesCompraPaginated
{
    /// <summary>
    /// Filtros calcados del script de búsqueda legacy: si viene Code, busca
    /// exacto; si no, filtra por rango de fecha. Más Proveedor/Estado/
    /// Usuario(CreatedBy)/FormaPago — mismos parámetros del script.
    /// </summary>
    public class GetOrdenesCompraPaginatedQuery : PagedRequest, IRequest<PagedResult<OrdenCompraResponse>>
    {
        public string? PlantaCode { get; set; }
        public string? Code { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? ProveedorCode { get; set; }
        public string? Estado { get; set; }
        public string? Usuario { get; set; }
        public string? FormaPagoCode { get; set; }
    }
}