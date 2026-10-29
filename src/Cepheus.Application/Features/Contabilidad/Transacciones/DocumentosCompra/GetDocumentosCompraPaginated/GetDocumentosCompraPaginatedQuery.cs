using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.GetDocumentosCompraPaginated
{
    /// <summary>
    /// Filtros calcados de co_sp_Listado_Doc_Compras (@tipo, @numero, @Codigo_Prv, @Codigo_Est): el valor
    /// 'T' = todos del legacy pasa a ser "filtro no enviado". El número es búsqueda exacta (como el SP).
    /// </summary>
    public class GetDocumentosCompraPaginatedQuery
        : PagedRequest, IRequest<PagedResult<DocumentoCompraListItemResponse>>
    {
        /// <summary>@tipo: tipo de comprobante.</summary>
        public string? ComprobantePagoCode { get; set; }
        /// <summary>@numero: número de documento (exacto).</summary>
        public string? NumeroDocumento { get; set; }
        /// <summary>@Codigo_Prv: proveedor.</summary>
        public string? ProveedorCode { get; set; }
        /// <summary>@Codigo_Est: Pendiente o Cancelado.</summary>
        public string? Estado { get; set; }
    }
}
