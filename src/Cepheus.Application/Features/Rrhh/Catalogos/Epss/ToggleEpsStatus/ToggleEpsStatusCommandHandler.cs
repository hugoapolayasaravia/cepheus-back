using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.ToggleEpsStatus
{
    public class ToggleEpsStatusCommandHandler : IRequestHandler<ToggleEpsStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleEpsStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleEpsStatusCommand request, CancellationToken cancellationToken)
        {
            var eps = await _uow.Rrhh.Catalogos.Epss.Query()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (eps is null)
            {
                throw new KeyNotFoundException($"EPS {request.Code} no encontrada.");
            }

            eps.IsActive = !eps.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return eps.IsActive;
        }
    }
}