using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.ToggleProveedorCondicionStatus
{
    public class ToggleProveedorCondicionStatusCommandHandler : IRequestHandler<ToggleProveedorCondicionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleProveedorCondicionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleProveedorCondicionStatusCommand request, CancellationToken cancellationToken)
        {
            var condicion = await _uow.Logistica.Maestros.ProveedorCondiciones.GetByIdAsync(request.Id, cancellationToken);

            if (condicion is null)
            {
                throw new KeyNotFoundException($"Condición {request.Id} no encontrada.");
            }

            condicion.IsActive = !condicion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return condicion.IsActive;
        }
    }
}
