using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.ToggleInspeccionStatus
{
    public class ToggleInspeccionStatusCommandHandler : IRequestHandler<ToggleInspeccionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleInspeccionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleInspeccionStatusCommand request, CancellationToken cancellationToken)
        {
            var inspeccion = await _uow.Mantenimiento.Catalogos.Inspecciones.Query()
                .FirstOrDefaultAsync(i => i.Code == request.Code, cancellationToken);

            if (inspeccion is null)
            {
                throw new KeyNotFoundException($"Inspección {request.Code} no encontrada.");
            }

            inspeccion.IsActive = !inspeccion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return inspeccion.IsActive;
        }
    }
}
