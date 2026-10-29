using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.AnularNotaCreditoVenta
{
    /// <summary>
    /// Anula la nota de crédito. Si nació de una solicitud, la solicitud vuelve a Pendiente
    /// (con sus etapas aprobadas) para poder generar una nueva nota.
    /// PENDIENTE de integrar (fuera del alcance): validación de cierre contable de la fecha
    /// (fg_cierre), anulación del asiento de provisión (fc_anulaasiento / ad_sp_anula_documento)
    /// y migración a Aquarius.
    /// </summary>
    public class AnularNotaCreditoVentaCommandHandler
        : IRequestHandler<AnularNotaCreditoVentaCommand, NotaCreditoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public AnularNotaCreditoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCreditoVentaResponse> Handle(
            AnularNotaCreditoVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaCreditoVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            var nota = await _uow.Facturacion.Transacciones.NotasCreditoVenta.Query()
                .FirstOrDefaultAsync(n => n.PlantaCode == plantaCode && n.Number == number, cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException(
                    $"Nota de crédito {NotaCreditoVentaRules.FormatKey(plantaCode, number)} no encontrada.");
            }

            if (nota.Status == EstadoNotaCredito.Anulado)
            {
                throw new InvalidOperationException("La nota de crédito ya está anulada.");
            }

            nota.Status = EstadoNotaCredito.Anulado;
            nota.CanceledAt = DateTime.UtcNow;
            nota.CancelReason = request.Reason.Trim();
            nota.PendingAmount = 0m;

            if (nota.SolicitudCode is not null && nota.SolicitudYear is not null && nota.SolicitudMonth is not null)
            {
                var solicitud = await _uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query()
                    .FirstOrDefaultAsync(s => s.Code == nota.SolicitudCode
                                              && s.Year == nota.SolicitudYear
                                              && s.Month == nota.SolicitudMonth, cancellationToken);

                if (solicitud is not null && solicitud.Status == EstadoSolicitudNotaCredito.Facturado)
                {
                    solicitud.Status = EstadoSolicitudNotaCredito.Pendiente;
                }
            }

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
