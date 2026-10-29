using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.CreateMotivoSolicitudNotaCredito
{
    public class CreateMotivoSolicitudNotaCreditoCommandHandler
        : IRequestHandler<CreateMotivoSolicitudNotaCreditoCommand, MotivoSolicitudNotaCreditoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateMotivoSolicitudNotaCreditoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoSolicitudNotaCreditoResponse> Handle(
            CreateMotivoSolicitudNotaCreditoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query().Select(t => t.Code),
                length: 2,
                entityLabel: "Motivos de Solicitud de Nota de Crédito",
                cancellationToken);

            var motivo = new MotivoSolicitudNotaCredito
            {
                Code = code,
                Name = request.Name.Trim(),
                ParentCode = string.IsNullOrWhiteSpace(request.ParentCode) ? null : request.ParentCode.Trim(),
                MotivoDevolucionCode = string.IsNullOrWhiteSpace(request.MotivoDevolucionCode) ? null : request.MotivoDevolucionCode.Trim(),
                AffectsStock = request.AffectsStock,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.AddAsync(motivo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return (await MotivoSolicitudNotaCreditoProjection.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
