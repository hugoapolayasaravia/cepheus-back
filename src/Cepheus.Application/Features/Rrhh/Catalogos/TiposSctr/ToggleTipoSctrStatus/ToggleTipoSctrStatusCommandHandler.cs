using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.ToggleTipoSctrStatus
{
    public class ToggleTipoSctrStatusCommandHandler : IRequestHandler<ToggleTipoSctrStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoSctrStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoSctrStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoSctr = await _uow.Rrhh.Catalogos.TiposSctr.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoSctr is null)
            {
                throw new KeyNotFoundException($"Tipo de SCTR {request.Code} no encontrado.");
            }

            tipoSctr.IsActive = !tipoSctr.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoSctr.IsActive;
        }
    }
}