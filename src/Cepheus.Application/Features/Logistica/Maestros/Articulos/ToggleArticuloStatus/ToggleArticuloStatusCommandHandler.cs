using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.ToggleArticuloStatus
{
    public class ToggleArticuloStatusCommandHandler : IRequestHandler<ToggleArticuloStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleArticuloStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleArticuloStatusCommand request, CancellationToken cancellationToken)
        {
            var articulo = await _uow.Logistica.Maestros.Articulos.Query()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (articulo is null)
            {
                throw new KeyNotFoundException($"Artículo {request.Code} no encontrado.");
            }

            articulo.IsActive = !articulo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return articulo.IsActive;
        }
    }
}
