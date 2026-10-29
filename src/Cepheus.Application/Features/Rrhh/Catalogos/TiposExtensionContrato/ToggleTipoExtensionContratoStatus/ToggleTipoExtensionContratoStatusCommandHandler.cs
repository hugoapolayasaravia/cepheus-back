using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.ToggleTipoExtensionContratoStatus
{
    public class ToggleTipoExtensionContratoStatusCommandHandler
        : IRequestHandler<ToggleTipoExtensionContratoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoExtensionContratoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoExtensionContratoStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoExtensionContrato = await _uow.Rrhh.Catalogos.TiposExtensionContrato.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoExtensionContrato is null)
            {
                throw new KeyNotFoundException($"Tipo de extensión de contrato {request.Code} no encontrado.");
            }

            tipoExtensionContrato.IsActive = !tipoExtensionContrato.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoExtensionContrato.IsActive;
        }
    }
}