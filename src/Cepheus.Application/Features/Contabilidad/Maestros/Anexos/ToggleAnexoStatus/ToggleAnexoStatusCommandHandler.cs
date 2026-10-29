using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.ToggleAnexoStatus
{
    public class ToggleAnexoStatusCommandHandler : IRequestHandler<ToggleAnexoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleAnexoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleAnexoStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Maestros.Anexos.Query()
                .FirstOrDefaultAsync(x => x.TipoAnexoCode == request.TipoAnexoCode && x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Anexo {request.TipoAnexoCode}/{request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
