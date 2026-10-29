using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.UpdateSolicitudNotaCredito
{
    public class UpdateSolicitudNotaCreditoCommandHandler
        : IRequestHandler<UpdateSolicitudNotaCreditoCommand, SolicitudNotaCreditoResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public UpdateSolicitudNotaCreditoCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<SolicitudNotaCreditoResponse> Handle(
            UpdateSolicitudNotaCreditoCommand request, CancellationToken cancellationToken)
        {
            var code = SolicitudNotaCreditoRules.Normalize(request.Code);
            var year = request.Year.Trim();
            var month = request.Month.Trim();
            var negocio = SolicitudNotaCreditoRules.Normalize(request.NegocioCode);

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var solicitud = await _uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query()
                        .Include(s => s.Details)
                        .Include(s => s.Guides)
                        .Include(s => s.Approvals)
                        .FirstOrDefaultAsync(s => s.Code == code && s.Year == year && s.Month == month, ct);

                    if (solicitud is null)
                    {
                        throw new KeyNotFoundException(
                            $"Solicitud de nota de crédito {SolicitudNotaCreditoRules.FormatKey(year, month, code)} no encontrada.");
                    }

                    if (!solicitud.RowVersion.SequenceEqual(request.RowVersion))
                    {
                        throw new InvalidOperationException(
                            "La solicitud fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
                    }

                    if (solicitud.Status != EstadoSolicitudNotaCredito.Pendiente || !solicitud.IsEditable)
                    {
                        throw new InvalidOperationException("La solicitud ya no es editable.");
                    }

                    if (solicitud.Approvals.Any(a => a.Status != EstadoAprobacionSolicitud.Pendiente))
                    {
                        throw new InvalidOperationException(
                            "La solicitud ya tiene etapas gestionadas y no puede modificarse.");
                    }

                    if (negocio != solicitud.NegocioCode)
                    {
                        throw new InvalidOperationException("No se puede cambiar el negocio de la solicitud.");
                    }

                    if (request.IssueDate.Year.ToString().PadLeft(4, '0') != year
                        || request.IssueDate.Month.ToString().PadLeft(2, '0') != month)
                    {
                        throw new InvalidOperationException(
                            "La fecha de emisión debe pertenecer al mismo año y mes de la solicitud (forman parte de su clave).");
                    }

                    await SolicitudNotaCreditoRules.EnsureUserCanAsync(
                        _uow, _currentUser, solicitud.NegocioCode, EtapaSolicitudNotaCredito.Ingresa, ct);

                    var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(_uow, request.IssueDate, ct);

                    // Reemplazo de líneas y guías: primero se eliminan, luego se insertan
                    // (las claves de las nuevas pueden coincidir con las anteriores).
                    foreach (var detail in solicitud.Details.ToList())
                    {
                        _uow.Facturacion.Transacciones.SolicitudesNotaCreditoDetalle.Remove(detail);
                    }

                    foreach (var guide in solicitud.Guides.ToList())
                    {
                        _uow.Facturacion.Transacciones.SolicitudesNotaCreditoGuias.Remove(guide);
                    }

                    await _uow.SaveChangesAsync(ct);

                    var (details, subTotal) = SolicitudNotaCreditoRules.BuildDetails(code, year, month, request.Details);
                    var guides = SolicitudNotaCreditoRules.BuildGuides(code, year, month, request.Guides);
                    var totals = DocumentoVentaRules.ComputeTotals(subTotal, igvPercentage);

                    solicitud.IssueDate = request.IssueDate;
                    solicitud.ClienteCode = request.ClienteCode.Trim();
                    solicitud.DocumentTypeCode = request.DocumentTypeCode.Trim();
                    solicitud.DocumentNumber = request.DocumentNumber.Trim();
                    solicitud.MotivoCode = request.MotivoCode.Trim();
                    solicitud.MonedaCode = SolicitudNotaCreditoRules.Normalize(request.MonedaCode);
                    solicitud.Observation = request.Observation.Trim();
                    solicitud.IsProcedent = request.IsProcedent;
                    solicitud.RequesterAccepts = request.RequesterAccepts;
                    solicitud.SubTotal = totals.SubTotal;
                    solicitud.IgvAmount = totals.IgvAmount;
                    solicitud.TotalAmount = totals.TotalAmount;

                    foreach (var detail in details)
                    {
                        await _uow.Facturacion.Transacciones.SolicitudesNotaCreditoDetalle.AddAsync(detail, ct);
                    }

                    foreach (var guide in guides)
                    {
                        await _uow.Facturacion.Transacciones.SolicitudesNotaCreditoGuias.AddAsync(guide, ct);
                    }

                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La solicitud fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await SolicitudNotaCreditoMapper.LoadAsync(_uow, year, month, code, cancellationToken))!;
        }
    }
}
