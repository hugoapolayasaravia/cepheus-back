using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.UpdateUsuarioSolicitudNotaCredito
{
    public class UpdateUsuarioSolicitudNotaCreditoCommandHandler
        : IRequestHandler<UpdateUsuarioSolicitudNotaCreditoCommand, UsuarioSolicitudNotaCreditoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUsuarioSolicitudNotaCreditoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UsuarioSolicitudNotaCreditoResponse> Handle(
            UpdateUsuarioSolicitudNotaCreditoCommand request, CancellationToken cancellationToken)
        {
            var negocioCode = request.NegocioCode.Trim().ToUpperInvariant();
            var stage = EtapaSolicitudNotaCreditoParser.Parse(request.Stage);

            var current = await _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.NegocioCode == negocioCode && u.Stage == stage && u.UserId == request.UserId,
                    cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException(
                    $"Usuario {request.UserId} no está asignado a la etapa {stage} del negocio {negocioCode}.");
            }

            var usuario = new UsuarioSolicitudNotaCredito
            {
                NegocioCode = negocioCode,
                Stage = stage,
                UserId = request.UserId,
                IsPrincipal = request.IsPrincipal,
                CanEnter = request.CanEnter,
                CanManage = request.CanManage,
                CanApprove = request.CanApprove,
                StartDate = request.StartDate,
                EndDate = request.EndDate,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Update(usuario);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La asignación fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await UsuarioSolicitudNotaCreditoProjection.LoadAsync(
                _uow, negocioCode, stage, request.UserId, cancellationToken))!;
        }
    }
}
