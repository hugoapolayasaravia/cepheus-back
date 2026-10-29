using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.ToggleSubFamiliaStatus
{
    public class ToggleSubFamiliaStatusCommandHandler : IRequestHandler<ToggleSubFamiliaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleSubFamiliaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleSubFamiliaStatusCommand request, CancellationToken cancellationToken)
        {
            var subFamilia = await _uow.Logistica.Catalogos.SubFamilias.Query()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (subFamilia is null)
            {
                throw new KeyNotFoundException($"SubFamilia {request.Code} no encontrada.");
            }

            subFamilia.IsActive = !subFamilia.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return subFamilia.IsActive;
        }
    }
}