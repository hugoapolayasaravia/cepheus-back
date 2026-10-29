using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.ToggleFallaStatus
{
    public class ToggleFallaStatusCommandHandler : IRequestHandler<ToggleFallaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleFallaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleFallaStatusCommand request, CancellationToken cancellationToken)
        {
            var falla = await _uow.Mantenimiento.Catalogos.Fallas.Query()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (falla is null)
            {
                throw new KeyNotFoundException($"Falla {request.Code} no encontrada.");
            }

            falla.IsActive = !falla.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return falla.IsActive;
        }
    }
}
