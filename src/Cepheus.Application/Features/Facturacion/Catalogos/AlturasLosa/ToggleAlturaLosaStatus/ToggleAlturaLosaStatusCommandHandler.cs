using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.ToggleAlturaLosaStatus
{
    public class ToggleAlturaLosaStatusCommandHandler : IRequestHandler<ToggleAlturaLosaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleAlturaLosaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleAlturaLosaStatusCommand request, CancellationToken cancellationToken)
        {
            var alturaLosa = await _uow.Facturacion.Catalogos.AlturasLosa.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (alturaLosa is null)
            {
                throw new KeyNotFoundException($"Altura de losa {request.Code} no encontrada.");
            }

            alturaLosa.IsActive = !alturaLosa.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return alturaLosa.IsActive;
        }
    }
}
