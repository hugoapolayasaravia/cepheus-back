using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.GetOTRMaquinasByOrdenTrabajo
{
    public class GetOTRMaquinasByOrdenTrabajoQueryHandler
        : IRequestHandler<GetOTRMaquinasByOrdenTrabajoQuery, List<OTRMaquinaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetOTRMaquinasByOrdenTrabajoQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<OTRMaquinaResponse>> Handle(
            GetOTRMaquinasByOrdenTrabajoQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant();

            return await _uow.Mantenimiento.Transacciones.OTRMaquinas.Query()
                .AsNoTracking()
                .Where(o => o.PlantaCode == plantaCode && o.OrdenTrabajoCode == ordenCode)
                .OrderBy(o => o.MaquinaCode)
                .Select(o => new OTRMaquinaResponse
                {
                    PlantaCode = o.PlantaCode,
                    OrdenTrabajoCode = o.OrdenTrabajoCode,
                    MaquinaCode = o.MaquinaCode,
                    FechaProceso = o.FechaProceso,
                    Cantidad = o.Cantidad,
                    Horas = o.Horas,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    RowVersion = o.RowVersion
                })
                .ToListAsync(cancellationToken);
        }
    }
}
