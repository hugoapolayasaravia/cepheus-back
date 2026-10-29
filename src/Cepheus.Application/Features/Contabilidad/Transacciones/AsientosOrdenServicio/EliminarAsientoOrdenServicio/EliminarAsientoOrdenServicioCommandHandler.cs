using System.Data;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.EliminarAsientoOrdenServicio
{
    /// <summary>
    /// Elimina el asiento de una orden de servicio (legacy: evento ue_eliminaasiento), en una sola transacción:
    ///   1. La orden debe tener asiento.
    ///   2. Si generó un documento de compra, solo se elimina si está Pendiente y sin pagos; de lo contrario se
    ///      rechaza ("tiene pagos").
    ///   3. Se anula el asiento (período abierto, no tipo 04) y se libera la referencia en la orden, su salida y las
    ///      líneas de la salida.
    /// </summary>
    public class EliminarAsientoOrdenServicioCommandHandler
        : IRequestHandler<EliminarAsientoOrdenServicioCommand, AsientoOrdenServicioResultResponse>
    {
        private readonly IUnitOfWork _uow;

        public EliminarAsientoOrdenServicioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<AsientoOrdenServicioResultResponse> Handle(
            EliminarAsientoOrdenServicioCommand request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var codigo = request.OrdenServicioCode.Trim().ToUpperInvariant();

            return _uow.ExecuteInTransactionAsync(
                token => EliminarAsync(planta, codigo, token),
                IsolationLevel.ReadCommitted,
                cancellationToken);
        }

        private async Task<AsientoOrdenServicioResultResponse> EliminarAsync(
            string planta, string codigo, CancellationToken ct)
        {
            var os = await _uow.Logistica.Transacciones.OrdenesServicio.Query()
                .FirstOrDefaultAsync(o => o.PlantaCode == planta && o.Code == codigo, ct)
                ?? throw new KeyNotFoundException($"Orden de servicio {planta}-{codigo} no encontrada.");

            var asientoCodigo = (os.AsientoContable ?? string.Empty).Trim();
            if (asientoCodigo.Length != AsientoContableRules.CodigoLength)
            {
                throw new InvalidOperationException("Asiento no generado ...");
            }

            string? documentoTipo = null;
            string? documentoNumero = null;

            if (!string.IsNullOrWhiteSpace(os.NumeroDocumento))
            {
                var numero = os.NumeroDocumento.Trim();

                var documento = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                    .Include(d => d.Detalles)
                    .FirstOrDefaultAsync(d =>
                        d.ComprobantePagoCode == os.ComprobantePagoCode
                        && d.NumeroDocumento == numero
                        && d.ProveedorCode == os.ProveedorCode, ct);

                if (documento is not null)
                {
                    if (documento.PagadoSol > 0 || documento.Estado != EstadoDocumentoCompra.Pendiente)
                    {
                        throw new InvalidOperationException(
                            $"Documento {documento.ComprobantePagoCode} {documento.NumeroDocumento} {documento.ProveedorCode} ... tiene pagos ...");
                    }

                    documentoTipo = documento.ComprobantePagoCode;
                    documentoNumero = documento.NumeroDocumento;
                    _uow.Contabilidad.Transacciones.DocumentosCompra.Remove(documento);
                }
            }

            await AsientoAnulacionService.AnularAsync(_uow, asientoCodigo, ct);

            await _uow.SaveChangesAsync(ct);

            return new AsientoOrdenServicioResultResponse
            {
                PlantaCode = planta,
                OrdenServicioCode = codigo,
                Success = true,
                AsientoCodigo = asientoCodigo,
                DocumentoComprobantePagoCode = documentoTipo,
                DocumentoNumero = documentoNumero,
                Message = $"Asiento {asientoCodigo} anulado."
            };
        }
    }
}
