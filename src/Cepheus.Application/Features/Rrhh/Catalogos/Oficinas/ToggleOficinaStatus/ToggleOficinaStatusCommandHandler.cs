using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.ToggleOficinaStatus
{
    public class ToggleOficinaStatusCommandHandler : IRequestHandler<ToggleOficinaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleOficinaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleOficinaStatusCommand request, CancellationToken cancellationToken)
        {
            var oficina = await _uow.Rrhh.Catalogos.Oficinas.Query()
                .FirstOrDefaultAsync(o => o.Code == request.Code, cancellationToken);

            if (oficina is null)
            {
                throw new KeyNotFoundException($"Oficina {request.Code} no encontrada.");
            }

            oficina.IsActive = !oficina.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return oficina.IsActive;
        }
    }
}