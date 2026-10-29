using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.ToggleTipoOrdenStatus
{
    public class ToggleTipoOrdenStatusCommandHandler : IRequestHandler<ToggleTipoOrdenStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoOrdenStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoOrdenStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoOrden = await _uow.Mantenimiento.Catalogos.TiposOrden.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoOrden is null)
            {
                throw new KeyNotFoundException($"Tipo de orden {request.Code} no encontrado.");
            }

            tipoOrden.IsActive = !tipoOrden.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoOrden.IsActive;
        }
    }
}
