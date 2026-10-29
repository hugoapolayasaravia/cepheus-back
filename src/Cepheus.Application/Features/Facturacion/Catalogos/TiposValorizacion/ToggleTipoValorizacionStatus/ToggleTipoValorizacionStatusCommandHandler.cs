using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.ToggleTipoValorizacionStatus
{
    public class ToggleTipoValorizacionStatusCommandHandler : IRequestHandler<ToggleTipoValorizacionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoValorizacionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoValorizacionStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoValorizacion = await _uow.Facturacion.Catalogos.TiposValorizacion.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoValorizacion is null)
            {
                throw new KeyNotFoundException($"Tipo de valorización {request.Code} no encontrado.");
            }

            tipoValorizacion.IsActive = !tipoValorizacion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoValorizacion.IsActive;
        }
    }
}
