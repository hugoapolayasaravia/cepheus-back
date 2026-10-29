using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.DeleteDocumentoCompraDetalle
{
    public class DeleteDocumentoCompraDetalleCommandHandler
        : IRequestHandler<DeleteDocumentoCompraDetalleCommand, DocumentoCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public DeleteDocumentoCompraDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DocumentoCompraResponse> Handle(
            DeleteDocumentoCompraDetalleCommand request, CancellationToken cancellationToken)
        {
            var tdo = request.ComprobantePagoCode.Trim().ToUpperInvariant();
            var prv = request.ProveedorCode.Trim().ToUpperInvariant();
            var numero = request.NumeroDocumento.Trim();

            var doc = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                .Include(d => d.Proveedor)
                .Include(d => d.Detalles)
                .FirstOrDefaultAsync(d => d.ComprobantePagoCode == tdo && d.ProveedorCode == prv && d.NumeroDocumento == numero,
                    cancellationToken);

            if (doc is null)
            {
                throw new KeyNotFoundException($"Documento {tdo} {numero} del proveedor {prv} no encontrado.");
            }

            DocumentoCompraEdicionRules.EnsureEditable(doc, "eliminar líneas");

            var detalle = doc.Detalles.FirstOrDefault(d => d.ItemNumber == request.ItemNumber);
            if (detalle is null)
            {
                throw new KeyNotFoundException(
                    $"La línea {request.ItemNumber} no existe en el documento {tdo} {numero}.");
            }

            if (doc.Detalles.Count == 1)
            {
                throw new InvalidOperationException("El documento debe conservar al menos una línea.");
            }

            doc.Detalles.Remove(detalle);
            _uow.Contabilidad.Transacciones.DocumentoCompraDetalles.Remove(detalle);

            await DocumentoCompraTotalsCalculator.RecalculateAsync(_uow, doc, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            var response = DocumentoCompraMapper.Map(doc);
            response.TipoCambioVenta = await DocumentoCompraTipoCambioResolver.GetVentaAsync(
                _uow, doc.MonedaCode, doc.TipoCambioFecha, cancellationToken);
            return response;
        }
    }
}
