using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GetCobranzasDudosasPaginated
{
    /// <summary>
    /// Listado de documentos en cobranza dudosa (legacy dw_det_cobranzadudosa / co_CobranzaDudosa: todos, o por tipo y
    /// documento). Estado: Registrado o Cancelado. <c>OnlyWithoutCanje</c> lista los pendientes de generar el asiento.
    /// </summary>
    public class GetCobranzasDudosasPaginatedQuery : PagedRequest, IRequest<PagedResult<CobranzaDudosaResponse>>
    {
        public string? TypeCode { get; set; }
        public string? Number { get; set; }
        public string? ClienteCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Status { get; set; }
        public bool? OnlyWithoutCanje { get; set; }
    }
}
