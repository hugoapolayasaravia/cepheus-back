using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.GetControlCierre
{
    public class GetControlCierreQueryHandler : IRequestHandler<GetControlCierreQuery, ControlCierreResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetControlCierreQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ControlCierreResponse> Handle(GetControlCierreQuery request, CancellationToken cancellationToken)
        {
            var control = await _uow.Logistica.Maestros.ControlCierres.Query()
                .AsNoTracking()
                .Where(c => c.PlantaCode == request.PlantaCode && c.PeriodCode == request.PeriodCode)
                .Select(c => new ControlCierreResponse
                {
                    PlantaCode = c.PlantaCode,
                    PeriodCode = c.PeriodCode,
                    ClosureDate = c.ClosureDate,
                    DifferenceAmount = c.DifferenceAmount,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (control is null)
            {
                throw new KeyNotFoundException(
                    $"No hay cierre registrado para planta {request.PlantaCode} en el período {request.PeriodCode}.");
            }

            return control;
        }
    }
}
