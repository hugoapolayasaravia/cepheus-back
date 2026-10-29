using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.ToggleTipoAfiliacionStatus
{
    public class ToggleTipoAfiliacionStatusCommandHandler : IRequestHandler<ToggleTipoAfiliacionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoAfiliacionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoAfiliacionStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoAfiliacion = await _uow.Rrhh.Catalogos.TiposAfiliacion.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoAfiliacion is null)
            {
                throw new KeyNotFoundException($"Tipo de afiliación {request.Code} no encontrado.");
            }

            tipoAfiliacion.IsActive = !tipoAfiliacion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoAfiliacion.IsActive;
        }
    }
}