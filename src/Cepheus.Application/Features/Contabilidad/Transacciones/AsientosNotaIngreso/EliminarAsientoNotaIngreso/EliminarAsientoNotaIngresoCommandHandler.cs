using System.Data;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.EliminarAsientoNotaIngreso
{
    /// <summary>
    /// Elimina el asiento de una nota de ingreso (legacy: evento ue_eliminaasiento), en una sola transacción:
    ///   1. La nota de ingreso debe tener asiento.
    ///   2. Si generó un documento de compra, solo se elimina si está Pendiente y sin pagos; de lo contrario se rechaza
    ///      ("tiene pagos").
    ///   3. Se anula el asiento (período abierto, no tipo 04) y se libera la referencia en la nota de ingreso.
    /// </summary>
    public class EliminarAsientoNotaIngresoCommandHandler
        : IRequestHandler<EliminarAsientoNotaIngresoCommand, AsientoNotaIngresoResultResponse>
    {
        private readonly IUnitOfWork _uow;

        public EliminarAsientoNotaIngresoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<AsientoNotaIngresoResultResponse> Handle(
            EliminarAsientoNotaIngresoCommand request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var codigo = request.NotaIngresoCode.Trim().ToUpperInvariant();

            return _uow.ExecuteInTransactionAsync(
                token => EliminarAsync(planta, codigo, token),
                IsolationLevel.ReadCommitted,
                cancellationToken);
        }

        private async Task<AsientoNotaIngresoResultResponse> EliminarAsync(
            string planta, string codigo, CancellationToken ct)
        {
            var ni = await _uow.Logistica.Transacciones.NotaIngresos.Query()
                .FirstOrDefaultAsync(n => n.PlantaCode == planta && n.Code == codigo, ct)
                ?? throw new KeyNotFoundException($"Nota de ingreso {planta}-{codigo} no encontrada.");

            var asientoCodigo = (ni.AsientoContable ?? string.Empty).Trim();
            if (asientoCodigo.Length != AsientoContableRules.CodigoLength)
            {
                throw new InvalidOperationException("Asiento no generado ...");
            }

            string? documentoTipo = null;
            string? documentoNumero = null;

            if (ni.Origen != OrigenNotaIngreso.Transferencia && !string.IsNullOrWhiteSpace(ni.NumeroDocumento))
            {
                var numero = ni.NumeroDocumento.Trim();

                var documento = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                    .Include(d => d.Detalles)
                    .FirstOrDefaultAsync(d =>
                        d.ComprobantePagoCode == ni.ComprobantePagoCode
                        && d.NumeroDocumento == numero
                        && d.ProveedorCode == ni.ProveedorCode, ct);

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

            ni.AsientoContable = null;

            await AsientoAnulacionService.AnularAsync(_uow, asientoCodigo, ct);

            await _uow.SaveChangesAsync(ct);

            return new AsientoNotaIngresoResultResponse
            {
                PlantaCode = planta,
                NotaIngresoCode = codigo,
                Success = true,
                AsientoCodigo = asientoCodigo,
                DocumentoComprobantePagoCode = documentoTipo,
                DocumentoNumero = documentoNumero,
                Message = $"Asiento {asientoCodigo} anulado."
            };
        }
    }
}
