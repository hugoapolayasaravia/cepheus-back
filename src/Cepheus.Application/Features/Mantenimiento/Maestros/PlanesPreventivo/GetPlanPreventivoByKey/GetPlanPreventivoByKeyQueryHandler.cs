using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.GetPlanPreventivoByKey
{
    public class GetPlanPreventivoByKeyQueryHandler : IRequestHandler<GetPlanPreventivoByKeyQuery, PlanPreventivoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetPlanPreventivoByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanPreventivoResponse> Handle(GetPlanPreventivoByKeyQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var result = await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                .AsNoTracking()
                .Where(x => x.PlantaCode == plantaCode && x.Code == code)
                .Select(PlanPreventivoProjection.ToResponse)
                .FirstOrDefaultAsync(cancellationToken);

            return result ?? throw new KeyNotFoundException($"Plan preventivo {plantaCode}/{code} no encontrado.");
        }
    }
}
