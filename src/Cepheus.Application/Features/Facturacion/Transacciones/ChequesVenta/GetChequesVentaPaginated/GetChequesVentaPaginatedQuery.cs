using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GetChequesVentaPaginated
{
    /// <summary>
    /// Búsqueda de cheques (legacy ad_consulta_cheques): por número de cheque, o por número de cancelación, o por
    /// rango de fecha de emisión + cliente + banco + estado.
    /// Si viene <c>Number</c> solo se busca por él; si no, si viene <c>CancelacionCode</c> solo por ese; si no, por el resto.
    /// </summary>
    public class GetChequesVentaPaginatedQuery : PagedRequest, IRequest<PagedResult<ChequeVentaListItemResponse>>
    {
        public string? Number { get; set; }
        public string? CancelacionCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? ClienteCode { get; set; }
        public string? BancoCode { get; set; }

        /// <summary>Pendiente, Rebotado, Cobrado, Anulado (o 1..4).</summary>
        public string? Status { get; set; }
    }
}
