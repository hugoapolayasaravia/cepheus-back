using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.EmitirNotaCreditoVenta
{
    /// <summary>
    /// Emite la nota de crédito (equivale a la impresión definitiva del legacy,
    /// ue_set_btn_imp): pasa a Facturado, marca impresión y fecha.
    /// PENDIENTE de integrar (fuera del alcance de este módulo): generación del asiento
    /// de provisión (fc_genasiprovision, se completa ProvisionEntryNumber) y suspensión de la
    /// OT en Programación (ue_suspendeot).
    /// </summary>
    public class EmitirNotaCreditoVentaCommandHandler
        : IRequestHandler<EmitirNotaCreditoVentaCommand, NotaCreditoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public EmitirNotaCreditoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCreditoVentaResponse> Handle(
            EmitirNotaCreditoVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaCreditoVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            var nota = await _uow.Facturacion.Transacciones.NotasCreditoVenta.Query()
                .Include(n => n.Details)
                .FirstOrDefaultAsync(n => n.PlantaCode == plantaCode && n.Number == number, cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException(
                    $"Nota de crédito {NotaCreditoVentaRules.FormatKey(plantaCode, number)} no encontrada.");
            }

            if (nota.Status != EstadoNotaCredito.Pendiente)
            {
                throw new InvalidOperationException($"La nota de crédito está {nota.Status}: solo se emite una Pendiente.");
            }

            if (nota.Details.Count == 0 || nota.TotalAmount <= 0)
            {
                throw new InvalidOperationException("La nota de crédito no tiene líneas o su total es cero.");
            }

            nota.Status = EstadoNotaCredito.Facturado;
            nota.IsPrinted = true;
            nota.PrintedAt = DateTime.UtcNow;

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La nota de crédito fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await NotaCreditoVentaMapper.LoadAsync(_uow, plantaCode, number, cancellationToken))!;
        }
    }
}
