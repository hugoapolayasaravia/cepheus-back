using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.ProtestarLetraVenta
{
    public class ProtestarLetraVentaCommandHandler : IRequestHandler<ProtestarLetraVentaCommand, LetraVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public ProtestarLetraVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LetraVentaResponse> Handle(ProtestarLetraVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = LetraVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var letra = await LetraVentaRules.GetTrackedAsync(_uow, plantaCode, number, ct);

                    LetraVentaRules.EnsureRowVersion(letra, request.RowVersion);

                    if (!LetraVentaRules.CanProtestar(letra))
                    {
                        throw new InvalidOperationException(
                            $"Solo se pueden protestar letras Pendientes o Canceladas. Estado actual: {letra.Status}.");
                    }

                    var applied = await CobranzaDocumentoService.GetAppliedAmountAsync(
                        _uow,
                        new CobranzaDocumento
                        {
                            TypeCode = CobranzaTipoDocumento.Letra,
                            PlantaCode = letra.PlantaCode,
                            Number = letra.Number,
                            ClienteCode = letra.ClienteCode
                        },
                        ct);

                    if (applied > 0)
                    {
                        throw new InvalidOperationException(
                            "La letra ya fue aplicada en otra cancelación: anule primero esa cancelación.");
                    }

                    letra.Status = EstadoLetra.Facturado;
                    letra.PendingAmount = letra.TotalAmount;
                    letra.SettledAt = null;

                    await _uow.SaveChangesAsync(ct);

                    // Legacy: ad_actualizacancelaciones('1') y ad_actualizadocumentos('1') de la cancelación origen.
                    await CobranzaDocumentoService.ActualizarCancelacionAsync(_uow, letra.CancelacionCode, apply: true, ct);

                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La letra fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await LetraVentaMapper.LoadAsync(_uow, plantaCode, number, cancellationToken))!;
        }
    }
}
