using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.ToggleTipoZonaStatus
{
    public class ToggleTipoZonaStatusCommandHandler : IRequestHandler<ToggleTipoZonaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoZonaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoZonaStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoZona = await _uow.Rrhh.Catalogos.TiposZona.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoZona is null)
            {
                throw new KeyNotFoundException($"Tipo de zona {request.Code} no encontrado.");
            }

            tipoZona.IsActive = !tipoZona.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoZona.IsActive;
        }
    }
}