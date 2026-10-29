using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.ToggleOportunidadStatus
{
    public class ToggleOportunidadStatusCommandHandler : IRequestHandler<ToggleOportunidadStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleOportunidadStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleOportunidadStatusCommand request, CancellationToken cancellationToken)
        {
            var oportunidad = await _uow.Mantenimiento.Catalogos.Oportunidades.Query()
                .FirstOrDefaultAsync(o => o.Code == request.Code, cancellationToken);

            if (oportunidad is null)
            {
                throw new KeyNotFoundException($"Oportunidad {request.Code} no encontrada.");
            }

            oportunidad.IsActive = !oportunidad.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return oportunidad.IsActive;
        }
    }
}
