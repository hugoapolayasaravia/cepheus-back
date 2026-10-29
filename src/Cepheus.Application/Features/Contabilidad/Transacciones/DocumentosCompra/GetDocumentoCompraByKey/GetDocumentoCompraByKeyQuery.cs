using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.GetDocumentoCompraByKey
{
    public record GetDocumentoCompraByKeyQuery(string ComprobantePagoCode, string ProveedorCode, string NumeroDocumento)
        : IRequest<DocumentoCompraResponse>;
}
