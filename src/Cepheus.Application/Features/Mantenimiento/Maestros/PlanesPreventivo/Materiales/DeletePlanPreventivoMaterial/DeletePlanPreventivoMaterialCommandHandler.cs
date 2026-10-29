using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.DeletePlanPreventivoMaterial
{
    public class DeletePlanPreventivoMaterialCommandHandler : IRequestHandler<DeletePlanPreventivoMaterialCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeletePlanPreventivoMaterialCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeletePlanPreventivoMaterialCommand request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var plan = request.PlanCode.Trim().ToUpperInvariant();
            var key = request.ArticuloCode.Trim().ToUpperInvariant();

            var entity = await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoMateriales.Query()
                .FirstOrDefaultAsync(d => d.PlantaCode == planta && d.PlanCode == plan && d.ArticuloCode == key, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Registro {planta}/{plan}/{key} no encontrado.");
            }

            _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoMateriales.Remove(entity);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
