using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.ToggleTipoArticuloStatus
{
    public class ToggleTipoArticuloStatusCommandHandler : IRequestHandler<ToggleTipoArticuloStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoArticuloStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoArticuloStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoArticulo = await _uow.Logistica.Catalogos.TiposArticulo.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoArticulo is null)
            {
                throw new KeyNotFoundException($"Tipo de artículo {request.Code} no encontrado.");
            }

            tipoArticulo.IsActive = !tipoArticulo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoArticulo.IsActive;
        }
    }
}