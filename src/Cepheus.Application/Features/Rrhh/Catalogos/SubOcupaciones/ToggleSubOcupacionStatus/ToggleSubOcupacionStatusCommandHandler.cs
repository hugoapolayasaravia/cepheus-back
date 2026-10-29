using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.ToggleSubOcupacionStatus
{
    public class ToggleSubOcupacionStatusCommandHandler : IRequestHandler<ToggleSubOcupacionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleSubOcupacionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleSubOcupacionStatusCommand request, CancellationToken cancellationToken)
        {
            var subOcupacion = await _uow.Rrhh.Catalogos.SubOcupaciones.Query()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (subOcupacion is null)
            {
                throw new KeyNotFoundException($"Sub ocupación {request.Code} no encontrada.");
            }

            subOcupacion.IsActive = !subOcupacion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return subOcupacion.IsActive;
        }
    }
}