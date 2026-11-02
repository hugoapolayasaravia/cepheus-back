using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.UpdateLetraVenta
{
    public class UpdateLetraVentaCommandHandler : IRequestHandler<UpdateLetraVentaCommand, LetraVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateLetraVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LetraVentaResponse> Handle(UpdateLetraVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = LetraVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var letra = await LetraVentaRules.GetTrackedAsync(_uow, plantaCode, number, ct);

                    LetraVentaRules.EnsureRowVersion(letra, request.RowVersion);

                    if (!LetraVentaRules.CanEdit(letra))
                    {
                        throw new InvalidOperationException("Solo se pueden modificar letras por cobrar (Facturado).");
                    }

                    if (letra.AcceptedByClient && !request.AcceptedByClient)
                    {
                        throw new InvalidOperationException("La letra ya fue aceptada por el cliente: no se puede quitar la aceptación.");
                    }

                    var accepting = !letra.AcceptedByClient && request.AcceptedByClient;

                    letra.IssueDate = request.IssueDate.Date;
                    letra.DueDate = request.DueDate.Date;
                    letra.AcceptedByClient = request.AcceptedByClient;
                    letra.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

                    await _uow.SaveChangesAsync(ct);

                    if (accepting && request.ExchangeDocuments)
                    {
                        // Canje: los documentos que la letra reemplaza quedan cancelados (legacy ad_actualizadocumentos '1').
                        var lines = await _uow.Facturacion.Transacciones.CancelacionesVentaDetalle.Query()
                            .AsNoTracking()
                            .Where(d => d.CancelacionCode == letra.CancelacionCode)
                            .ToListAsync(ct);

                        foreach (var line in lines)
                        {
                            await CobranzaDocumentoService.ActualizarAsync(
                                _uow, line.TypeCode, line.PlantaCode, line.DocumentNumber, line.ClienteCode,
                                line.DocumentBancoCode, apply: true, letra.SettledAt, ct);
                        }

                        await _uow.SaveChangesAsync(ct);
                    }

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
