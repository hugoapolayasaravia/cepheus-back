using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.UpdateDocumentoCompra
{
    public class UpdateDocumentoCompraCommandHandler : IRequestHandler<UpdateDocumentoCompraCommand, DocumentoCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateDocumentoCompraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DocumentoCompraResponse> Handle(
            UpdateDocumentoCompraCommand request, CancellationToken cancellationToken)
        {
            var tdo = request.ComprobantePagoCode.Trim().ToUpperInvariant();
            var numero = request.NumeroDocumento.Trim();
            var prv = request.ProveedorCode.Trim().ToUpperInvariant();

            var doc = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                .Include(d => d.Proveedor)
                .Include(d => d.Detalles)
                .FirstOrDefaultAsync(d => d.ComprobantePagoCode == tdo && d.NumeroDocumento == numero && d.ProveedorCode == prv,
                    cancellationToken);

            if (doc is null)
            {
                throw new KeyNotFoundException($"Documento {tdo} {numero} del proveedor {prv} no encontrado.");
            }

            if (doc.Estado != EstadoDocumentoCompra.Pendiente)
            {
                throw new InvalidOperationException(
                    $"El documento está en estado '{doc.Estado}' y ya no admite edición.");
            }

            if (doc.PagadoSol > 0)
            {
                throw new InvalidOperationException("El documento ya tiene pagos registrados y no admite edición.");
            }

            if (!string.IsNullOrWhiteSpace(doc.AsientoProvisionCode))
            {
                throw new InvalidOperationException(
                    $"El documento ya generó el asiento de provisión {doc.AsientoProvisionCode} y no admite edición.");
            }

            var monedaCode = request.MonedaCode.Trim().ToUpperInvariant();
            var esSoles = monedaCode == DocumentoCompraRules.MonedaSoles;

            doc.FechaEmision = request.FechaEmision.Date;
            doc.FechaRecepcion = request.FechaRecepcion.Date;
            doc.FechaDocumento = request.FechaDocumento?.Date;
            doc.FechaVencimiento = request.FechaVencimiento.Date;
            doc.FormaPagoCode = request.FormaPagoCode.Trim().ToUpperInvariant();
            doc.MonedaCode = monedaCode;
            doc.TipoCambioFecha = esSoles ? null : request.TipoCambioFecha;
            doc.NoGravableSol = request.NoGravableSol;
            doc.ServicioSol = request.ServicioSol;
            doc.IgvExteriorSol = request.IgvExteriorSol;
            doc.Glosa = request.Glosa.Trim();
            doc.Origen = System.Enum.Parse<OrigenDocumentoCompra>(request.Origen, true);
            doc.Procedencia = string.IsNullOrWhiteSpace(request.Procedencia)
                ? null
                : System.Enum.Parse<ProcedenciaDocumentoCompra>(request.Procedencia, true);
            doc.TipoBienCode = Clean(request.TipoBienCode);
            doc.TipoOperacionCode = Clean(request.TipoOperacionCode);
            doc.OperacionDetraccion = request.OperacionDetraccion?.Trim();
            doc.ConstanciaDetraccion = request.ConstanciaDetraccion?.Trim();
            doc.FechaConstanciaDetraccion = request.FechaConstanciaDetraccion?.Date;
            doc.NumeroDetraccion = request.NumeroDetraccion?.Trim();
            doc.FechaEmisionDetraccion = request.FechaEmisionDetraccion?.Date;
            doc.MontoDetraccionSol = request.MontoDetraccionSol;
            doc.DocumentoOrigenComprobantePagoCode = Clean(request.DocumentoOrigenComprobantePagoCode);
            doc.DocumentoOrigenNumero = string.IsNullOrWhiteSpace(request.DocumentoOrigenNumero)
                ? null
                : request.DocumentoOrigenNumero.Trim();
            doc.OrdenCompraAdministracionCode = Clean(request.OrdenCompraAdministracionCode);
            doc.RowVersion = request.RowVersion;

            // Cambia la fórmula (fecha de emisión, montos digitados): se recalcula.
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
