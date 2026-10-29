using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.ToggleSexoStatus
{
    public class ToggleSexoStatusCommandHandler : IRequestHandler<ToggleSexoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleSexoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleSexoStatusCommand request, CancellationToken cancellationToken)
        {
            var sexo = await _uow.Rrhh.Catalogos.Sexos.Query()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (sexo is null)
            {
                throw new KeyNotFoundException($"Sexo {request.Code} no encontrado.");
            }

            sexo.IsActive = !sexo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return sexo.IsActive;
        }
    }
}