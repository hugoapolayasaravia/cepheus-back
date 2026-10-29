using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.ToggleUnidadNegocioStatus
{
    public class ToggleUnidadNegocioStatusCommandHandler : IRequestHandler<ToggleUnidadNegocioStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleUnidadNegocioStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleUnidadNegocioStatusCommand request, CancellationToken cancellationToken)
        {
            var unidad = await _uow.Logistica.Catalogos.UnidadesNegocio.Query()
                .FirstOrDefaultAsync(u => u.Code == request.Code, cancellationToken);

            if (unidad is null)
            {
                throw new KeyNotFoundException($"Unidad de negocio {request.Code} no encontrada.");
            }

            unidad.IsActive = !unidad.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return unidad.IsActive;
        }
    }
}