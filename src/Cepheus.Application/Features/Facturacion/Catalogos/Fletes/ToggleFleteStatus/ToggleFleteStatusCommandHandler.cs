using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.ToggleFleteStatus
{
    public class ToggleFleteStatusCommandHandler : IRequestHandler<ToggleFleteStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleFleteStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleFleteStatusCommand request, CancellationToken cancellationToken)
        {
            var flete = await _uow.Facturacion.Catalogos.Fletes.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (flete is null)
            {
                throw new KeyNotFoundException($"Flete {request.Code} no encontrado.");
            }

            flete.IsActive = !flete.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return flete.IsActive;
        }
    }
}
