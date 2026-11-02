using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetrasVentaPaginated
{
    /// <summary>
    /// Búsqueda de letras (legacy ad_consulta_letras): por número de letra, o por número de cancelación, o por
    /// rango de fecha de emisión + cliente + banco + estado. Con <c>Number</c> solo se busca por él; si no, con
    /// <c>CancelacionCode</c> solo por ese.
    /// </summary>
    public class GetLetrasVentaPaginatedQuery : PagedRequest, IRequest<PagedResult<LetraVentaListItemResponse>>
    {
        public string? Number { get; set; }
        public string? CancelacionCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? ClienteCode { get; set; }
        public string? CobradorCode { get; set; }
        public string? BancoCode { get; set; }

        /// <summary>Pendiente, Facturado, Cancelado, Anulado (o 1..4).</summary>
        public string? Status { get; set; }
    }
}
