using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.CastigarCobranzaDudosa
{
    public class CastigarCobranzaDudosaCommandHandler
        : IRequestHandler<CastigarCobranzaDudosaCommand, CobranzaDudosaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CastigarCobranzaDudosaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CobranzaDudosaResponse> Handle(
            CastigarCobranzaDudosaCommand request, CancellationToken cancellationToken)
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

                    if (cd.WrittenOffAt.HasValue)
                    {
                        throw new InvalidOperationException($"El documento {type} {number} ya fue castigado.");
                    }

                    if (!CancelacionVentaRules.IsAsiento(cd.CanjeEntry))
                    {
                        throw new InvalidOperationException(
                            $"Genere primero el asiento de canje del documento {type} {number}.");
                    }

                    var doc = await CobranzaDudosaRules.LoadDocumentAsync(_uow, cd, ct);

                    // Legacy: CobranzaDudosa = 'X' y Fecha_Cas = fecha del servidor.
                    doc.SetDoubtful(EstadoCobranzaDudosa.Castigada, doc.DoubtfulExchangeRate);
                    cd.WrittenOffAt = DateTime.Today;

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
