using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Negocios.ToggleNegocioStatus
{
    public class ToggleNegocioStatusCommandHandler : IRequestHandler<ToggleNegocioStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleNegocioStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleNegocioStatusCommand request, CancellationToken cancellationToken)
        {
            var negocio = await _uow.Comunes.Negocios.Query()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (negocio is null)
            {
                throw new KeyNotFoundException($"Negocio {request.Code} no encontrado.");
            }

            negocio.IsActive = !negocio.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return negocio.IsActive;
        }
    }
}
