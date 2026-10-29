using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.ToggleEstadoCivilStatus
{
    public class ToggleEstadoCivilStatusCommandHandler : IRequestHandler<ToggleEstadoCivilStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleEstadoCivilStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleEstadoCivilStatusCommand request, CancellationToken cancellationToken)
        {
            var estadoCivil = await _uow.Rrhh.Catalogos.EstadosCiviles.Query()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (estadoCivil is null)
            {
                throw new KeyNotFoundException($"Estado civil {request.Code} no encontrado.");
            }

            estadoCivil.IsActive = !estadoCivil.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return estadoCivil.IsActive;
        }
    }
}