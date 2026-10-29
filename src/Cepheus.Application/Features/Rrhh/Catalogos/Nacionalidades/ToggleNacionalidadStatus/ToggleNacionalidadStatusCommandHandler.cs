using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.ToggleNacionalidadStatus
{
    public class ToggleNacionalidadStatusCommandHandler : IRequestHandler<ToggleNacionalidadStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleNacionalidadStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleNacionalidadStatusCommand request, CancellationToken cancellationToken)
        {
            var nacionalidad = await _uow.Rrhh.Catalogos.Nacionalidades.Query()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (nacionalidad is null)
            {
                throw new KeyNotFoundException($"Nacionalidad {request.Code} no encontrada.");
            }

            nacionalidad.IsActive = !nacionalidad.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return nacionalidad.IsActive;
        }
    }
}