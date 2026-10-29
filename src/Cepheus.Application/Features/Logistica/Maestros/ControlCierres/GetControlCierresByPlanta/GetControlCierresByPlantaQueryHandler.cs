using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.GetControlCierresByPlanta
{
    public class GetControlCierresByPlantaQueryHandler
        : IRequestHandler<GetControlCierresByPlantaQuery, List<ControlCierreResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetControlCierresByPlantaQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ControlCierreResponse>> Handle(
            GetControlCierresByPlantaQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            return await _uow.Logistica.Maestros.ControlCierres.Query()
                .AsNoTracking()
                .Where(c => c.PlantaCode == plantaCode)
                .OrderByDescending(c => c.PeriodCode)
                .Select(c => new ControlCierreResponse
                {
                    PlantaCode = c.PlantaCode,
                    PeriodCode = c.PeriodCode,
                    ClosureDate = c.ClosureDate,
                    DifferenceAmount = c.DifferenceAmount,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedBy = c.UpdatedBy
                })
                .ToListAsync(cancellationToken);
        }
    }
}
