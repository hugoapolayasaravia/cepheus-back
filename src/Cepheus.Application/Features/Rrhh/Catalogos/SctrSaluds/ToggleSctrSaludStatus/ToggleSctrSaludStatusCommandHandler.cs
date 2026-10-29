using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.ToggleSctrSaludStatus
{
    public class ToggleSctrSaludStatusCommandHandler : IRequestHandler<ToggleSctrSaludStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleSctrSaludStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleSctrSaludStatusCommand request, CancellationToken cancellationToken)
        {
            var sctrSalud = await _uow.Rrhh.Catalogos.SctrsSalud.Query()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (sctrSalud is null)
            {
                throw new KeyNotFoundException($"Cobertura de salud SCTR {request.Code} no encontrada.");
            }

            sctrSalud.IsActive = !sctrSalud.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return sctrSalud.IsActive;
        }
    }
}