using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.ReabrirCajaChica
{
    public class ReabrirCajaChicaCommandHandler : IRequestHandler<ReabrirCajaChicaCommand, CajaChicaResponse>
    {
        private readonly IUnitOfWork _uow;

        public ReabrirCajaChicaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CajaChicaResponse> Handle(ReabrirCajaChicaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var caja = await _uow.Contabilidad.Transacciones.CajasChica.Query()
                        .Include(c => c.Details)
                        .FirstOrDefaultAsync(c => c.Code == code, ct);

                    if (caja is null)
                    {
                        throw new KeyNotFoundException($"Caja chica {code} no encontrada.");
                    }

                    if (caja.Status != EstadoCajaChica.Procesado && caja.Status != EstadoCajaChica.Cancelado)
                    {
                        throw new InvalidOperationException(
                            $"La caja chica está {caja.Status}: solo se reabre una caja Procesada o Cancelada.");
                    }

                    // Primero la reversión y luego la provisión (anular el asiento libera documentos,
                    // notas de ingreso, órdenes de servicio y los códigos de asiento de la caja).
                    if (!string.IsNullOrWhiteSpace(caja.ReversionAsientoCode))
                    {
                        await AsientoAnulacionService.AnularAsync(_uow, caja.ReversionAsientoCode.Trim(), ct);
                    }

                    if (!string.IsNullOrWhiteSpace(caja.ProvisionAsientoCode))
                    {
                        await AsientoAnulacionService.AnularAsync(_uow, caja.ProvisionAsientoCode.Trim(), ct);
                    }

                    // Documentos de compra generados por esta caja
                    foreach (var d in caja.Details.Where(x => x.ComprobantePagoCode != CajaChicaRules.ComprobanteSinDocumento))
                    {
                        var documento = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                            .Include(x => x.Detalles)
                            .FirstOrDefaultAsync(x => x.ComprobantePagoCode == d.ComprobantePagoCode
                                                      && x.NumeroDocumento == d.NumeroDocumento
                                                      && x.ProveedorCode == d.ProveedorCode
                                                      && x.Origen == OrigenDocumentoCompra.CajaChica, ct);

                        if (documento is null)
                        {
                            continue;
                        }

                        foreach (var det in documento.Detalles.ToList())
                        {
                            _uow.Contabilidad.Transacciones.DocumentoCompraDetalles.Remove(det);
                        }

                        _uow.Contabilidad.Transacciones.DocumentosCompra.Remove(documento);
                    }

                    caja.ProvisionAsientoCode = null;
                    caja.ReversionAsientoCode = null;
                    caja.IsPrinted = false;
                    caja.Status = EstadoCajaChica.Pendiente;

                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La caja chica fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await CajaChicaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
