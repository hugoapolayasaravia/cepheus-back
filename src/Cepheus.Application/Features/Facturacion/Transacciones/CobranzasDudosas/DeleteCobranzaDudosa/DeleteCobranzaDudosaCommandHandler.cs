using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.DeleteCobranzaDudosa
{
    public class DeleteCobranzaDudosaCommandHandler : IRequestHandler<DeleteCobranzaDudosaCommand, Unit>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCobranzaDudosaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Unit> Handle(DeleteCobranzaDudosaCommand request, CancellationToken cancellationToken)
        {
            var type = CobranzaDudosaRules.NormalizeType(request.TypeCode);
            var number = request.Number.Trim();

            CobranzaDudosaRules.EnsureReference(type, number, request.PlantaCode, request.ClienteCode, request.BancoCode);

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var cd = await CobranzaDudosaRules.GetTrackedAsync(
                        _uow, type, request.PlantaCode, number, request.ClienteCode, request.BancoCode, ct);

                    // Legacy: estado 'XX' = el documento ya fue cancelado por cobranza dudosa.
                    if (cd.Status == EstadoRegistroCobranzaDudosa.Cancelado)
                    {
                        throw new InvalidOperationException($"El documento {type} {number} ya fue cancelado por cobranza dudosa.");
                    }

                    // Mejora sobre el legacy: no se deshace si el documento ya se cobró como dudoso (la cancelación
                    // tomó la cuenta 1291 del asiento de canje).
                    var planta = cd.PlantaCode;
                    var banco = cd.BancoCode;

                    var cobrado = await _uow.Facturacion.Transacciones.CancelacionesVentaDetalle.Query().AsNoTracking()
                        .AnyAsync(d => d.TypeCode == cd.TypeCode && d.DocumentNumber == cd.Number
                                       && d.ClienteCode == cd.ClienteCode && d.IsDoubtful
                                       && (planta == "" || d.PlantaCode == planta)
                                       && (banco == "" || d.DocumentBancoCode == banco)
                                       && d.Cancelacion.Status != EstadoCancelacion.Anulado, ct);

                    if (cobrado)
                    {
                        throw new InvalidOperationException(
                            $"El documento {type} {number} tiene cancelaciones aplicadas como cobranza dudosa: anúlelas primero.");
                    }

                    var conCanje = CancelacionVentaRules.IsAsiento(cd.CanjeEntry);

                    if (conCanje)
                    {
                        // Legacy fc_anulaasiento(cje,'',cje): período abierto (el extorno de mes cerrado no está implementado).
                        await AsientoAnulacionService.AnularAsync(_uow, cd.CanjeEntry!.Trim(), ct);
                    }

                    var doc = await CobranzaDudosaRules.LoadDocumentAsync(_uow, cd, ct);

                    // Legacy: cobranzadudosa = 'N' y, con canje, restaura el asiento de provisión original.
                    doc.SetDoubtful(EstadoCobranzaDudosa.Normal, 0m);
                    if (conCanje)
                    {
                        doc.SetProvisionEntry(cd.ProvisionEntry);
                    }

                    _uow.Facturacion.Transacciones.CobranzasDudosas.Remove(cd);

                    await _uow.SaveChangesAsync(ct);
                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cobranza dudosa o el documento fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return Unit.Value;
        }
    }
}
