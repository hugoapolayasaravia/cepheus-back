using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.ToggleNivelStatus
{
    public class ToggleNivelStatusCommandHandler : IRequestHandler<ToggleNivelStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleNivelStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleNivelStatusCommand request, CancellationToken cancellationToken)
        {
            var nivel = await _uow.Logistica.Catalogos.Niveles.Query()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (nivel is null)
            {
                throw new KeyNotFoundException($"Nivel {request.Code} no encontrado.");
            }

            nivel.IsActive = !nivel.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return nivel.IsActive;
        }
    }
}
