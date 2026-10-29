using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.ToggleCentroCostoStatus
{
    public class ToggleCentroCostoStatusCommandHandler : IRequestHandler<ToggleCentroCostoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleCentroCostoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleCentroCostoStatusCommand request, CancellationToken cancellationToken)
        {
            var centro = await _uow.Logistica.Maestros.CentrosCosto.Query()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (centro is null)
            {
                throw new KeyNotFoundException($"Centro de costo {request.Code} no encontrado.");
            }

            centro.IsActive = !centro.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return centro.IsActive;
        }
    }
}
