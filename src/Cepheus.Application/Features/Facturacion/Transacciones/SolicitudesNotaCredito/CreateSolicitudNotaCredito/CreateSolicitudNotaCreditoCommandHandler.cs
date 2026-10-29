using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.CreateSolicitudNotaCredito
{
    public class CreateSolicitudNotaCreditoCommandHandler
        : IRequestHandler<CreateSolicitudNotaCreditoCommand, SolicitudNotaCreditoResponse>
    {
        private const int MaxAttempts = 3;

        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public CreateSolicitudNotaCreditoCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<SolicitudNotaCreditoResponse> Handle(
            CreateSolicitudNotaCreditoCommand request, CancellationToken cancellationToken)
        {
            var negocio = SolicitudNotaCreditoRules.Normalize(request.NegocioCode);

            // Quien registra debe estar habilitado en la etapa "Ingresa" del negocio.
            await SolicitudNotaCreditoRules.EnsureUserCanAsync(
                _uow, _currentUser, negocio, EtapaSolicitudNotaCredito.Ingresa, cancellationToken);

            var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(_uow, request.IssueDate, cancellationToken);

            var year = request.IssueDate.Year.ToString().PadLeft(4, '0');
            var month = request.IssueDate.Month.ToString().PadLeft(2, '0');

            for (var attempt = 1; ; attempt++)
            {
                var code = await SolicitudNotaCreditoRules.NextCodeAsync(_uow, year, cancellationToken);

                var (details, subTotal) = SolicitudNotaCreditoRules.BuildDetails(code, year, month, request.Details);
                var totals = DocumentoVentaRules.ComputeTotals(subTotal, igvPercentage);

                var solicitud = new SolicitudNotaCredito
                {
                    Code = code,
                    Year = year,
                    Month = month,
                    NegocioCode = negocio,
                    IssueDate = request.IssueDate,
                    ClienteCode = request.ClienteCode.Trim(),
                    DocumentTypeCode = request.DocumentTypeCode.Trim(),
                    DocumentNumber = request.DocumentNumber.Trim(),
                    MotivoCode = request.MotivoCode.Trim(),
                    MonedaCode = SolicitudNotaCreditoRules.Normalize(request.MonedaCode),
                    SubTotal = totals.SubTotal,
                    IgvAmount = totals.IgvAmount,
                    TotalAmount = totals.TotalAmount,
                    Observation = request.Observation.Trim(),
                    Status = EstadoSolicitudNotaCredito.Pendiente,
                    IsEditable = true,
                    IsProcedent = request.IsProcedent,
                    RequesterUserId = _currentUser.UserId,
                    RequesterAccepts = request.RequesterAccepts,
                    Details = details,
                    Guides = SolicitudNotaCreditoRules.BuildGuides(code, year, month, request.Guides),
                    Approvals = SolicitudNotaCreditoRules.BuildApprovals(code, year, month)
                };

                await _uow.Facturacion.Transacciones.SolicitudesNotaCredito.AddAsync(solicitud, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException) when (attempt < MaxAttempts)
                {
                    // Correlativo tomado por otro usuario en paralelo: reintenta con el siguiente.
                    _uow.ClearTracking();
                    continue;
                }

                _uow.ClearTracking();

                return (await SolicitudNotaCreditoMapper.LoadAsync(_uow, year, month, code, cancellationToken))!;
            }
        }
    }
}
