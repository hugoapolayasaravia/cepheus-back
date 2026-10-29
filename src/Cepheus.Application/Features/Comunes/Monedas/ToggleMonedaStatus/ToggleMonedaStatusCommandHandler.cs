using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Monedas.ToggleMonedaStatus
{
    public class ToggleMonedaStatusCommandHandler : IRequestHandler<ToggleMonedaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleMonedaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleMonedaStatusCommand request, CancellationToken cancellationToken)
        {

            var moneda = await _uow.Comunes.Monedas.Query()
                    .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (moneda is null)
            {
                throw new KeyNotFoundException($"Moneda {request.Code} no encontrada.");
            }

            moneda.IsActive = !moneda.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return moneda.IsActive;
        }
    }
}
