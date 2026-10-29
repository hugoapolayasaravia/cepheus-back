using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.ToggleTramiteStatus
{
    public class ToggleTramiteStatusCommandHandler : IRequestHandler<ToggleTramiteStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTramiteStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTramiteStatusCommand request, CancellationToken cancellationToken)
        {
            var tramite = await _uow.Logistica.Catalogos.Tramites.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tramite is null)
            {
                throw new KeyNotFoundException($"Trámite {request.Code} no encontrado.");
            }

            tramite.IsActive = !tramite.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tramite.IsActive;
        }
    }
}