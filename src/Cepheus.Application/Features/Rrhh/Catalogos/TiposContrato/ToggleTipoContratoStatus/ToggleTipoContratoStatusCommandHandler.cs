using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.ToggleTipoContratoStatus
{
    public class ToggleTipoContratoStatusCommandHandler : IRequestHandler<ToggleTipoContratoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoContratoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoContratoStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoContrato = await _uow.Rrhh.Catalogos.TiposContrato.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoContrato is null)
            {
                throw new KeyNotFoundException($"Tipo de contrato {request.Code} no encontrado.");
            }

            tipoContrato.IsActive = !tipoContrato.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoContrato.IsActive;
        }
    }
}