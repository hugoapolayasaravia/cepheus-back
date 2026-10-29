using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.AnularNotaDebitoVenta
{
    public class AnularNotaDebitoVentaCommandHandler
        : IRequestHandler<AnularNotaDebitoVentaCommand, NotaDebitoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public AnularNotaDebitoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaDebitoVentaResponse> Handle(
            AnularNotaDebitoVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaDebitoVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var nota = await _uow.Facturacion.Transacciones.NotasDebitoVenta.Query()
                        .Include(n => n.Details)
                        .FirstOrDefaultAsync(n => n.PlantaCode == plantaCode && n.Number == number, ct);

                    if (nota is null)
                    {
                        throw new KeyNotFoundException(
                            $"Nota de débito {NotaDebitoVentaRules.FormatKey(plantaCode, number)} no encontrada.");
                    }

                    if (nota.Status == EstadoNotaDebito.Anulado)
                    {
                        throw new InvalidOperationException("La nota de débito ya está anulada.");
                    }

                    if (nota.Status == EstadoNotaDebito.Cancelado || nota.PendingAmount != nota.TotalAmount)
                    {
                        throw new InvalidOperationException(
                            "La nota de débito tiene cancelaciones aplicadas: anule primero las cancelaciones.");
                    }

                    // Asiento de provisión (legacy: fc_anulaasiento). El período debe estar abierto.
                    if (!string.IsNullOrWhiteSpace(nota.ProvisionEntryNumber) && nota.ProvisionEntryNumber.Trim().Length == 13)
                    {
                        await AsientoAnulacionService.AnularAsync(_uow, nota.ProvisionEntryNumber.Trim(), ct);
                    }

                    // Fecha de anulación (legacy: ad_sp_anula_documento): hoy, o el último día del mes de emisión
                    // si la nota es de un mes anterior.
                    var now = DateTime.UtcNow;
                    var issue = nota.IssueDate.Date;
                    var canceledAt = issue.Year == now.Year && issue.Month == now.Month
                        ? now
                        : new DateTime(issue.Year, issue.Month, 1).AddMonths(1).AddDays(-1);

                    nota.Status = EstadoNotaDebito.Anulado;
                    nota.CanceledAt = canceledAt;
                    nota.CancelReason = request.Reason.Trim();
                    nota.PendingAmount = 0m;

                    // El SP elimina las líneas (delete from ddebventas).
                    foreach (var line in nota.Details.ToList())
                    {
                        _uow.Facturacion.Transacciones.NotasDebitoVentaDetalle.Remove(line);
                    }

                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La nota de débito fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await NotaDebitoVentaMapper.LoadAsync(_uow, plantaCode, number, cancellationToken))!;
        }
    }
}
