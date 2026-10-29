using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.ToggleTransportistaStatus
{
    public class ToggleTransportistaStatusCommandHandler : IRequestHandler<ToggleTransportistaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTransportistaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTransportistaStatusCommand request, CancellationToken cancellationToken)
        {
            var transportista = await _uow.Facturacion.Maestros.Transportistas.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (transportista is null)
            {
                throw new KeyNotFoundException($"Transportista {request.Code} no encontrado.");
            }

            transportista.IsActive = !transportista.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return transportista.IsActive;
        }
    }
}
