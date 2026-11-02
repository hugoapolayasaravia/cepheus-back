using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.AnularPagoProveedor
{
    public class AnularPagoProveedorCommandHandler : IRequestHandler<AnularPagoProveedorCommand, PagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public AnularPagoProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagoProveedorResponse> Handle(
            AnularPagoProveedorCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var pago = await _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (pago is null)
            {
                throw new KeyNotFoundException($"Pago {code} no encontrado.");
            }

            if (pago.Estado == EstadoPagoProveedor.Anulado)
            {
                throw new InvalidOperationException("Cheque ya se encuentra Anulado ...");
            }

            var asientoCode = (pago.AsientoCancelacionCode ?? string.Empty).Trim();
            var tieneAsiento = asientoCode.Length == PagoProveedorRules.AsientoLength;

            var anularOperacion = request.AnularOperacion;
            var anularAsiento = request.AnularAsiento;
            var liberarCheque = request.LiberarCheque;

            if (anularOperacion)
            {
                // Legacy: al anular la operación el asiento y el cheque se anulan siempre.
                anularAsiento = tieneAsiento;
                liberarCheque = true;
            }
            else
            {
                if (anularAsiento && !tieneAsiento)
                {
                    throw new InvalidOperationException("El pago no tiene asiento de cancelación para anular.");
                }

                if (liberarCheque && tieneAsiento && !anularAsiento)
                {
                    throw new InvalidOperationException("Para liberar el cheque primero debe anular el asiento de cancelación.");
                }

                if (liberarCheque && string.IsNullOrWhiteSpace(pago.NumeroCheque))
                {
                    throw new InvalidOperationException("El pago no tiene número de cheque para liberar.");
                }
            }

            if (anularOperacion)
            {
                // Legacy ue_eli_lin_det + actualiza_doc(..., 'B'): se quitan los documentos y cada uno recupera su saldo.
                var lineas = pago.Detalles.ToList();
                var absPorDocumento = lineas
                    .GroupBy(l => PagoProveedorRecalculator.Key(l))
                    .ToDictionary(g => g.Key, g => g.Sum(l => Math.Abs(l.PagoSol)));

                foreach (var (key, abs) in absPorDocumento)
                {
                    var linea = lineas.First(l => PagoProveedorRecalculator.Key(l) == key);
                    var doc = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                        .FirstOrDefaultAsync(d => d.ComprobantePagoCode == linea.ComprobantePagoCode
                                                  && d.NumeroDocumento == linea.NumeroDocumento
                                                  && d.ProveedorCode == linea.ProveedorCode,
                            cancellationToken);

                    if (doc is not null)
                    {
                        PagoProveedorRecalculator.ApplyDocumentBalance(doc, abs, 0m, pago.FechaEmision);
                    }
                }

                foreach (var linea in lineas)
                {
                    pago.Detalles.Remove(linea);
                    _uow.Contabilidad.Transacciones.PagoProveedorDetalles.Remove(linea);
                }

                pago.Estado = EstadoPagoProveedor.Anulado;
                pago.FechaAnulacion = DateTime.UtcNow.Date;
            }

            if (anularAsiento && tieneAsiento)
            {
                // Libera también la referencia en el pago (ver AsientoAnulacionService).
                await AsientoAnulacionService.AnularAsync(_uow, asientoCode, cancellationToken);

                pago.AsientoCancelacionCode = null;
                pago.ComprobanteCajaBancosImpreso = false;

                if (!anularOperacion)
                {
                    pago.Estado = EstadoPagoProveedor.Pendiente;
                }
            }

            if (liberarCheque)
            {
                pago.NumeroCheque = string.Empty;
            }

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El pago o un documento fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();
            return (await PagoProveedorMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
