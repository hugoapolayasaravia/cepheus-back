using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.ToggleEspecialidadStatus
{
    public class ToggleEspecialidadStatusCommandHandler : IRequestHandler<ToggleEspecialidadStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleEspecialidadStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleEspecialidadStatusCommand request, CancellationToken cancellationToken)
        {
            var especialidad = await _uow.Mantenimiento.Catalogos.Especialidades.Query()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (especialidad is null)
            {
                throw new KeyNotFoundException($"Especialidad {request.Code} no encontrada.");
            }

            especialidad.IsActive = !especialidad.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return especialidad.IsActive;
        }
    }
}
