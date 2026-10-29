using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.DeleteUsuarioPlanta
{
    public class DeleteUsuarioPlantaCommandHandler : IRequestHandler<DeleteUsuarioPlantaCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteUsuarioPlantaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteUsuarioPlantaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            var entity = await _uow.Logistica.Catalogos.UsuariosPlanta.Query()
                .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.PlantaCode == plantaCode, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException(
                    $"Asignación usuario/planta {request.UserId}/{plantaCode} no encontrada.");
            }

            _uow.Logistica.Catalogos.UsuariosPlanta.Remove(entity);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
