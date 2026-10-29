using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.GetDocumentoCompraByKey
{
    public class GetDocumentoCompraByKeyQueryHandler : IRequestHandler<GetDocumentoCompraByKeyQuery, DocumentoCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetDocumentoCompraByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DocumentoCompraResponse> Handle(
            GetDocumentoCompraByKeyQuery request, CancellationToken cancellationToken)
        {
            var tdo = request.ComprobantePagoCode.Trim().ToUpperInvariant();
            var prv = request.ProveedorCode.Trim().ToUpperInvariant();
            var numero = request.NumeroDocumento.Trim();

            var doc = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                .AsNoTracking()
                .Include(d => d.Proveedor)
                .Include(d => d.Detalles)
                .FirstOrDefaultAsync(d => d.ComprobantePagoCode == tdo && d.ProveedorCode == prv && d.NumeroDocumento == numero,
                    cancellationToken);

            if (doc is null)
            {
                throw new KeyNotFoundException($"Documento {tdo} {numero} del proveedor {prv} no encontrado.");
            }

            var response = DocumentoCompraMapper.Map(doc);
            response.PendienteSol = doc.PendienteSol;
            response.TipoCambioVenta = await DocumentoCompraTipoCambioResolver.GetVentaAsync(
                _uow, doc.MonedaCode, doc.TipoCambioFecha, cancellationToken);
            return response;
        }
    }
}
