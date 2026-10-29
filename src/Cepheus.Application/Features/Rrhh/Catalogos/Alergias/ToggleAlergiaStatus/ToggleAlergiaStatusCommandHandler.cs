using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.ToggleAlergiaStatus
{
    public class ToggleAlergiaStatusCommandHandler : IRequestHandler<ToggleAlergiaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleAlergiaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleAlergiaStatusCommand request, CancellationToken cancellationToken)
        {
            var alergia = await _uow.Rrhh.Catalogos.Alergias.Query()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (alergia is null)
            {
                throw new KeyNotFoundException($"Alergia {request.Code} no encontrada.");
            }

            alergia.IsActive = !alergia.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return alergia.IsActive;
        }
    }
}