using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.CreateDocumentoCompraDetalle
{
    public class CreateDocumentoCompraDetalleCommandHandler
        : IRequestHandler<CreateDocumentoCompraDetalleCommand, DocumentoCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateDocumentoCompraDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DocumentoCompraResponse> Handle(
            CreateDocumentoCompraDetalleCommand request, CancellationToken cancellationToken)
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

            DocumentoCompraEdicionRules.EnsureEditable(doc, "agregar líneas");

            var nextItem = doc.Detalles.Count == 0 ? 1 : doc.Detalles.Max(d => d.ItemNumber) + 1;

            var detalle = new DocumentoCompraDetalle
            {
                ComprobantePagoCode = tdo,
                ProveedorCode = prv,
                NumeroDocumento = numero,
                ItemNumber = nextItem,
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                CuentaContableCode = request.CuentaContableCode.Trim().ToUpperInvariant(),
                AnexoTipoCode = Clean(request.AnexoTipoCode),
                AnexoCode = Clean(request.AnexoCode),
                Detalle = request.Detalle.Trim(),
                Cantidad = request.Cantidad,
                UnidadMedidaCode = request.UnidadMedidaCode.Trim().ToUpperInvariant(),
                PrecioSol = request.PrecioSol,
                ParcialSol = DocumentoCompraTotalsCalculator.CalculateLineTotal(request.Cantidad, request.PrecioSol),
                NumeroGuiaVenta = request.NumeroGuiaVenta?.Trim(),
                ProductoTipoCode = Clean(request.ProductoTipoCode),
                ProductoCode = Clean(request.ProductoCode)
            };

            doc.Detalles.Add(detalle);
            await _uow.Contabilidad.Transacciones.DocumentoCompraDetalles.AddAsync(detalle, cancellationToken);

            await DocumentoCompraTotalsCalculator.RecalculateAsync(_uow, doc, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            var response = DocumentoCompraMapper.Map(doc);
            response.TipoCambioVenta = await DocumentoCompraTipoCambioResolver.GetVentaAsync(
                _uow, doc.MonedaCode, doc.TipoCambioFecha, cancellationToken);
            return response;
        }

        private static string? Clean(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
