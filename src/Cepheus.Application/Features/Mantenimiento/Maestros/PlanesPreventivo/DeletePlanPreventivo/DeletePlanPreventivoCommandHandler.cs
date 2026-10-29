using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.DeletePlanPreventivo
{
    public class DeletePlanPreventivoCommandHandler : IRequestHandler<DeletePlanPreventivoCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeletePlanPreventivoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeletePlanPreventivoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var entity = await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Code == code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Plan preventivo {plantaCode}/{code} no encontrado.");
            }

            _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Remove(entity);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
