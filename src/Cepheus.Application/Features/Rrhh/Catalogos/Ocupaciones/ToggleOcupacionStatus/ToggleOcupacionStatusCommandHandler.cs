using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.ToggleOcupacionStatus
{
    public class ToggleOcupacionStatusCommandHandler : IRequestHandler<ToggleOcupacionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleOcupacionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleOcupacionStatusCommand request, CancellationToken cancellationToken)
        {
            var ocupacion = await _uow.Rrhh.Catalogos.Ocupaciones.Query()
                .FirstOrDefaultAsync(o => o.Code == request.Code, cancellationToken);

            if (ocupacion is null)
            {
                throw new KeyNotFoundException($"Ocupación {request.Code} no encontrada.");
            }

            ocupacion.IsActive = !ocupacion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return ocupacion.IsActive;
        }
    }
}