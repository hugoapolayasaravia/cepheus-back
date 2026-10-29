using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.ToggleSituacionEpsStatus
{
    public class ToggleSituacionEpsStatusCommandHandler : IRequestHandler<ToggleSituacionEpsStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleSituacionEpsStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleSituacionEpsStatusCommand request, CancellationToken cancellationToken)
        {
            var situacionEps = await _uow.Rrhh.Catalogos.SituacionesEps.Query()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (situacionEps is null)
            {
                throw new KeyNotFoundException($"Situación EPS {request.Code} no encontrada.");
            }

            situacionEps.IsActive = !situacionEps.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return situacionEps.IsActive;
        }
    }
}