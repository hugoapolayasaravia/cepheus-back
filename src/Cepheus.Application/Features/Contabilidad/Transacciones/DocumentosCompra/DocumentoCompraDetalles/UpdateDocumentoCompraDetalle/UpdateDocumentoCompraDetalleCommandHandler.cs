using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.UpdateDocumentoCompraDetalle
{
    public class UpdateDocumentoCompraDetalleCommandHandler
        : IRequestHandler<UpdateDocumentoCompraDetalleCommand, DocumentoCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateDocumentoCompraDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DocumentoCompraResponse> Handle(
            UpdateDocumentoCompraDetalleCommand request, CancellationToken cancellationToken)
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

            DocumentoCompraEdicionRules.EnsureEditable(doc, "modificar líneas");

            var detalle = doc.Detalles.FirstOrDefault(d => d.ItemNumber == request.ItemNumber);
            if (detalle is null)
            {
                throw new KeyNotFoundException(
                    $"La línea {request.ItemNumber} no existe en el documento {tdo} {numero}.");
            }

            detalle.PlantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            detalle.CuentaContableCode = request.CuentaContableCode.Trim().ToUpperInvariant();
            detalle.AnexoTipoCode = Clean(request.AnexoTipoCode);
            detalle.AnexoCode = Clean(request.AnexoCode);
            detalle.Detalle = request.Detalle.Trim();
            detalle.Cantidad = request.Cantidad;
            detalle.UnidadMedidaCode = request.UnidadMedidaCode.Trim().ToUpperInvariant();
            detalle.PrecioSol = request.PrecioSol;
            detalle.ParcialSol = DocumentoCompraTotalsCalculator.CalculateLineTotal(request.Cantidad, request.PrecioSol);
            detalle.NumeroGuiaVenta = request.NumeroGuiaVenta?.Trim();
            detalle.ProductoTipoCode = Clean(request.ProductoTipoCode);
            detalle.ProductoCode = Clean(request.ProductoCode);

            await DocumentoCompraTotalsCalculator.RecalculateAsync(_uow, doc, cancellationToken);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El documento fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            var response = DocumentoCompraMapper.Map(doc);
            response.TipoCambioVenta = await DocumentoCompraTipoCambioResolver.GetVentaAsync(
                _uow, doc.MonedaCode, doc.TipoCambioFecha, cancellationToken);
            return response;
        }

        private static string? Clean(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
