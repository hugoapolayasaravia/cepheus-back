using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.ToggleLugarEnvioStatus
{
    public class ToggleLugarEnvioStatusCommandHandler : IRequestHandler<ToggleLugarEnvioStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleLugarEnvioStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleLugarEnvioStatusCommand request, CancellationToken cancellationToken)
        {
            var lugar = await _uow.Logistica.Catalogos.LugaresEnvio.Query()
                .FirstOrDefaultAsync(l => l.Code == request.Code, cancellationToken);

            if (lugar is null)
            {
                throw new KeyNotFoundException($"Lugar de envío {request.Code} no encontrado.");
            }

            lugar.IsActive = !lugar.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return lugar.IsActive;
        }
    }
}