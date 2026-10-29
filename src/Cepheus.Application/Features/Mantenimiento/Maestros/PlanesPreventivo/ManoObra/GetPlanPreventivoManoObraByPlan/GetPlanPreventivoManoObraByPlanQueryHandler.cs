using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.GetPlanPreventivoManoObraByPlan
{
    public class GetPlanPreventivoManoObraByPlanQueryHandler : IRequestHandler<GetPlanPreventivoManoObraByPlanQuery, List<PlanPreventivoManoObraResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPlanPreventivoManoObraByPlanQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<PlanPreventivoManoObraResponse>> Handle(GetPlanPreventivoManoObraByPlanQuery request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var plan = request.PlanCode.Trim().ToUpperInvariant();

            return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoManoObra.Query()
                .AsNoTracking()
                .Where(d => d.PlantaCode == planta && d.PlanCode == plan)
                .OrderBy(d => d.TrabajadorCode)
                .Select(PlanPreventivoManoObraProjection.ToResponse)
                .ToListAsync(cancellationToken);
        }
    }
}
