using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.CreatePlanPreventivoManoObra
{
    public class CreatePlanPreventivoManoObraCommandHandler : IRequestHandler<CreatePlanPreventivoManoObraCommand, PlanPreventivoManoObraResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePlanPreventivoManoObraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanPreventivoManoObraResponse> Handle(CreatePlanPreventivoManoObraCommand request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var plan = request.PlanCode.Trim().ToUpperInvariant();
            var key = request.TrabajadorCode.Trim().ToUpperInvariant();

            var entity = new PlanMantenimientoPreventivoManoObra
            {
                PlantaCode = planta,
                PlanCode = plan,
                TrabajadorCode = key,
                Tiempo = request.Tiempo,
                CostoTotal = request.CostoTotal
            };

            await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoManoObra.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoManoObra.Query()
                .AsNoTracking()
                .Where(d => d.PlantaCode == planta && d.PlanCode == plan && d.TrabajadorCode == key)
                .Select(PlanPreventivoManoObraProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
