using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.ToggleParentescoStatus
{
    public class ToggleParentescoStatusCommandHandler : IRequestHandler<ToggleParentescoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleParentescoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleParentescoStatusCommand request, CancellationToken cancellationToken)
        {
            var parentesco = await _uow.Rrhh.Catalogos.Parentescos.Query()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (parentesco is null)
            {
                throw new KeyNotFoundException($"Parentesco {request.Code} no encontrado.");
            }

            parentesco.IsActive = !parentesco.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return parentesco.IsActive;
        }
    }
}