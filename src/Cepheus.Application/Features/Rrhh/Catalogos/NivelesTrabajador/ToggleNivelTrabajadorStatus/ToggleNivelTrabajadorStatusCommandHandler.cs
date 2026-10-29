using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.ToggleNivelTrabajadorStatus
{
    public class ToggleNivelTrabajadorStatusCommandHandler
        : IRequestHandler<ToggleNivelTrabajadorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleNivelTrabajadorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleNivelTrabajadorStatusCommand request, CancellationToken cancellationToken)
        {
            var nivelTrabajador = await _uow.Rrhh.Catalogos.NivelesTrabajador.Query()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (nivelTrabajador is null)
            {
                throw new KeyNotFoundException($"Nivel {request.Code} no encontrado.");
            }

            nivelTrabajador.IsActive = !nivelTrabajador.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return nivelTrabajador.IsActive;
        }
    }
}