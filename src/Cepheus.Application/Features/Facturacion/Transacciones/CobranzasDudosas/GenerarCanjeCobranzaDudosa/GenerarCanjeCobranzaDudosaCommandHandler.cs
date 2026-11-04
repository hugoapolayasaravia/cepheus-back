using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GenerarCanjeCobranzaDudosa
{
    public class GenerarCanjeCobranzaDudosaCommandHandler
        : IRequestHandler<GenerarCanjeCobranzaDudosaCommand, CobranzaDudosaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GenerarCanjeCobranzaDudosaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CobranzaDudosaResponse> Handle(
            GenerarCanjeCobranzaDudosaCommand request, CancellationToken cancellationToken)
        {
            var type = CobranzaDudosaRules.NormalizeType(request.TypeCode);
            var number = request.Number.Trim();

            CobranzaDudosaRules.EnsureReference(type, number, request.PlantaCode, request.ClienteCode, request.BancoCode);

            string clienteCode = string.Empty;

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var cd = await CobranzaDudosaRules.GetTrackedAsync(
                        _uow, type, request.PlantaCode, number, request.ClienteCode, request.BancoCode, ct);
                    CobranzaDudosaRules.EnsureRowVersion(cd, request.RowVersion);

                    clienteCode = cd.ClienteCode;

                    if (CancelacionVentaRules.IsAsiento(cd.CanjeEntry))
                    {
                        throw new InvalidOperationException(
                            $"El asiento de canje del documento {type} {number} ya fue generado ({cd.CanjeEntry}).");
                    }

                    if (cd.Status != EstadoRegistroCobranzaDudosa.Registrado)
                    {
                        throw new InvalidOperationException($"La cobranza dudosa del documento {type} {number} está {cd.Status}.");
                    }

                    var doc = await CobranzaDudosaRules.LoadDocumentAsync(_uow, cd, ct);

                    if (doc.DoubtfulStatus != EstadoCobranzaDudosa.Dudosa)
                    {
                        throw new InvalidOperationException($"El documento {type} {number} no está en cobranza dudosa.");
                    }

                    var asiento = await CobranzaDudosaAsientoService.BuildAsync(_uow, cd, ct);
                    var canje = CancelacionVentaRules.AsientoCode(asiento);

                    cd.CanjeEntry = canje;

                    // Legacy: "cambio el asiento de provisión por el de canje". La cancelación posterior del documento
                    // toma su cuenta 1291 de este asiento.
                    doc.SetProvisionEntry(canje);

                    await _uow.SaveChangesAsync(ct);
                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cobranza dudosa o el documento fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return await CobranzaDudosaMapper.LoadAsync(
                _uow, type, request.PlantaCode, number, clienteCode, request.BancoCode, cancellationToken);
        }
    }
}
