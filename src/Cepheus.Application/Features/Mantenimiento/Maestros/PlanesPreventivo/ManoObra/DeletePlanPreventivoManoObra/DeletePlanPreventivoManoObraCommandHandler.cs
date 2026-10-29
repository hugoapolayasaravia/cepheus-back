using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.DeletePlanPreventivoManoObra
{
    public class DeletePlanPreventivoManoObraCommandHandler : IRequestHandler<DeletePlanPreventivoManoObraCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeletePlanPreventivoManoObraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeletePlanPreventivoManoObraCommand request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var plan = request.PlanCode.Trim().ToUpperInvariant();
            var key = request.TrabajadorCode.Trim().ToUpperInvariant();

            var entity = await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoManoObra.Query()
                .FirstOrDefaultAsync(d => d.PlantaCode == planta && d.PlanCode == plan && d.TrabajadorCode == key, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Registro {planta}/{plan}/{key} no encontrado.");
            }

            _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoManoObra.Remove(entity);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
