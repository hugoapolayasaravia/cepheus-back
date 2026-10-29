using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Modulos.ToggleModuloStatus
{
    public class ToggleModuloStatusCommandHandler : IRequestHandler<ToggleModuloStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleModuloStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleModuloStatusCommand request, CancellationToken cancellationToken)
        {
            var modulo = await _uow.Administracion.Modulos.GetByIdAsync(request.Id, cancellationToken);

            if (modulo is null)
            {
                throw new KeyNotFoundException($"Módulo {request.Id} no encontrado.");
            }

            modulo.IsActive = !modulo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return modulo.IsActive;
        }
    }

}
