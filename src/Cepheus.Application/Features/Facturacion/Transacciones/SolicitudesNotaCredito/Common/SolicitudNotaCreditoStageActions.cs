using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Domain.Facturacion.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common
{
    /// <summary>
    /// Lógica común de aprobar / rechazar una etapa de la solicitud.
    /// Reglas:
    ///  - La solicitud debe estar Pendiente.
    ///  - La etapa debe ser 01..05 y estar Pendiente.
    ///  - Las etapas anteriores deben estar Aprobadas (flujo secuencial).
    ///  - El usuario debe estar asignado a la etapa del negocio con la facultad que exige.
    ///  - Aprobar: la etapa queda Aprobada. Rechazar: la etapa queda Rechazada y la solicitud
    ///    pasa a Cancelado (exige observación).
    ///  - Tras la primera gestión la solicitud deja de ser editable.
    /// </summary>
    public static class SolicitudNotaCreditoStageActions
    {
        public static async Task<SolicitudNotaCreditoResponse> ExecuteAsync(
            IUnitOfWork uow,
            ICurrentUserService currentUser,
            string year, string month, string code,
            string stageText, string? observation, bool approve,
            CancellationToken cancellationToken)
        {
            var codeNorm = SolicitudNotaCreditoRules.Normalize(code);
            var stage = EtapaSolicitudNotaCreditoParser.Parse(stageText);

            var solicitud = await uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query()
                .Include(s => s.Approvals)
                .FirstOrDefaultAsync(s => s.Code == codeNorm && s.Year == year && s.Month == month, cancellationToken);

            if (solicitud is null)
            {
                throw new KeyNotFoundException(
                    $"Solicitud de nota de crédito {SolicitudNotaCreditoRules.FormatKey(year, month, codeNorm)} no encontrada.");
            }

            if (solicitud.Status != EstadoSolicitudNotaCredito.Pendiente)
            {
                throw new InvalidOperationException("Solo se puede gestionar una solicitud en estado Pendiente.");
            }

            if (stage == EtapaSolicitudNotaCredito.Ingresa)
            {
                throw new InvalidOperationException(
                    "La etapa Ingresa corresponde al registro de la solicitud y no se aprueba ni se rechaza.");
            }

            var approval = solicitud.Approvals.FirstOrDefault(a => a.Stage == stage);

            if (approval is null)
            {
                throw new InvalidOperationException($"La solicitud no tiene registrada la etapa {stage}.");
            }

            if (approval.Status != EstadoAprobacionSolicitud.Pendiente)
            {
                throw new InvalidOperationException($"La etapa {stage} ya fue gestionada ({approval.Status}).");
            }

            var previousNotApproved = solicitud.Approvals
                .Where(a => a.Stage < stage && a.Status != EstadoAprobacionSolicitud.Aprobado)
                .OrderBy(a => a.Stage)
                .FirstOrDefault();

            if (previousNotApproved is not null)
            {
                throw new InvalidOperationException(
                    $"Debe aprobarse primero la etapa {previousNotApproved.Stage}.");
            }

            await SolicitudNotaCreditoRules.EnsureUserCanAsync(
                uow, currentUser, solicitud.NegocioCode, stage, cancellationToken);

            approval.Status = approve ? EstadoAprobacionSolicitud.Aprobado : EstadoAprobacionSolicitud.Rechazado;
            approval.ActedByUserId = currentUser.UserId;
            approval.ActedBy = currentUser.FullName;
            approval.ActedAt = DateTime.UtcNow;
            approval.Observation = string.IsNullOrWhiteSpace(observation) ? null : observation.Trim();

            solicitud.IsEditable = false;

            if (!approve)
            {
                solicitud.Status = EstadoSolicitudNotaCredito.Cancelado;
            }

            try
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La solicitud fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            uow.ClearTracking();

            return (await SolicitudNotaCreditoMapper.LoadAsync(uow, year, month, codeNorm, cancellationToken))!;
        }
    }
}
