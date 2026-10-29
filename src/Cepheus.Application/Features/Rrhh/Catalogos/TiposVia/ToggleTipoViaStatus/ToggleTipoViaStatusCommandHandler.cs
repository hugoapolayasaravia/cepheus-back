using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.ToggleTipoViaStatus
{
    public class ToggleTipoViaStatusCommandHandler : IRequestHandler<ToggleTipoViaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoViaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoViaStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoVia = await _uow.Rrhh.Catalogos.TiposVia.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoVia is null)
            {
                throw new KeyNotFoundException($"Tipo de vía {request.Code} no encontrado.");
            }

            tipoVia.IsActive = !tipoVia.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoVia.IsActive;
        }
    }
}