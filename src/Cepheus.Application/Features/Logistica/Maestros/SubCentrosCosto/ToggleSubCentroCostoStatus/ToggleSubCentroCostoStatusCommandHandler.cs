using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.ToggleSubCentroCostoStatus
{
    public class ToggleSubCentroCostoStatusCommandHandler : IRequestHandler<ToggleSubCentroCostoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleSubCentroCostoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleSubCentroCostoStatusCommand request, CancellationToken cancellationToken)
        {
            var subCentro = await _uow.Logistica.Maestros.SubCentrosCosto.Query()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (subCentro is null)
            {
                throw new KeyNotFoundException($"SubCentro de costo {request.Code} no encontrado.");
            }

            subCentro.IsActive = !subCentro.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return subCentro.IsActive;
        }
    }
}
