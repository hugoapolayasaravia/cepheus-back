using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.ToggleTipoTrabajadorStatus
{
    public class ToggleTipoTrabajadorStatusCommandHandler : IRequestHandler<ToggleTipoTrabajadorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoTrabajadorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoTrabajadorStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoTrabajador = await _uow.Rrhh.Catalogos.TiposTrabajador.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoTrabajador is null)
            {
                throw new KeyNotFoundException($"Tipo de trabajador {request.Code} no encontrado.");
            }

            tipoTrabajador.IsActive = !tipoTrabajador.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoTrabajador.IsActive;
        }
    }
}