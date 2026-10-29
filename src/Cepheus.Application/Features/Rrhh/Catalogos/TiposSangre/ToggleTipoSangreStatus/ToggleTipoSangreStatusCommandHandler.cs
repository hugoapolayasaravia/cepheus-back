using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.ToggleTipoSangreStatus
{
    public class ToggleTipoSangreStatusCommandHandler : IRequestHandler<ToggleTipoSangreStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoSangreStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoSangreStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoSangre = await _uow.Rrhh.Catalogos.TiposSangre.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoSangre is null)
            {
                throw new KeyNotFoundException($"Tipo de sangre {request.Code} no encontrado.");
            }

            tipoSangre.IsActive = !tipoSangre.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoSangre.IsActive;
        }
    }
}
