using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.ToggleAfpStatus
{
    public class ToggleAfpStatusCommandHandler : IRequestHandler<ToggleAfpStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleAfpStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleAfpStatusCommand request, CancellationToken cancellationToken)
        {
            var afp = await _uow.Rrhh.Catalogos.Afps.Query()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (afp is null)
            {
                throw new KeyNotFoundException($"AFP {request.Code} no encontrada.");
            }

            afp.IsActive = !afp.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return afp.IsActive;
        }
    }
}