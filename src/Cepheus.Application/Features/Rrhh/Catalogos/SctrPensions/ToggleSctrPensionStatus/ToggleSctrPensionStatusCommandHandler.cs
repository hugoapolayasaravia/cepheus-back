using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.ToggleSctrPensionStatus
{
    public class ToggleSctrPensionStatusCommandHandler : IRequestHandler<ToggleSctrPensionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleSctrPensionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleSctrPensionStatusCommand request, CancellationToken cancellationToken)
        {
            var sctrPension = await _uow.Rrhh.Catalogos.SctrsPension.Query()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (sctrPension is null)
            {
                throw new KeyNotFoundException($"Cobertura de pensión SCTR {request.Code} no encontrada.");
            }

            sctrPension.IsActive = !sctrPension.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return sctrPension.IsActive;
        }
    }
}