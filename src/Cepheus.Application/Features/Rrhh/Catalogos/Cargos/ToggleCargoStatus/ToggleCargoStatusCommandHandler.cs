using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.ToggleCargoStatus
{
    public class ToggleCargoStatusCommandHandler : IRequestHandler<ToggleCargoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleCargoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleCargoStatusCommand request, CancellationToken cancellationToken)
        {
            var cargo = await _uow.Rrhh.Catalogos.Cargos.Query()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (cargo is null)
            {
                throw new KeyNotFoundException($"Cargo {request.Code} no encontrado.");
            }

            cargo.IsActive = !cargo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return cargo.IsActive;
        }
    }
}