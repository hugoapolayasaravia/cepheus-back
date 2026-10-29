using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.GetPlanPreventivoMaterialByPlan
{
    public class GetPlanPreventivoMaterialByPlanQueryHandler : IRequestHandler<GetPlanPreventivoMaterialByPlanQuery, List<PlanPreventivoMaterialResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPlanPreventivoMaterialByPlanQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<PlanPreventivoMaterialResponse>> Handle(GetPlanPreventivoMaterialByPlanQuery request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var plan = request.PlanCode.Trim().ToUpperInvariant();

            return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoMateriales.Query()
                .AsNoTracking()
                .Where(d => d.PlantaCode == planta && d.PlanCode == plan)
                .OrderBy(d => d.ArticuloCode)
                .Select(PlanPreventivoMaterialProjection.ToResponse)
                .ToListAsync(cancellationToken);
        }
    }
}
