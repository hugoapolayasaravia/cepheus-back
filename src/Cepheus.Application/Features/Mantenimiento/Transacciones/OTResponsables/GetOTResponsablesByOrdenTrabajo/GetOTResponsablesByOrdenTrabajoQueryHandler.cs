using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.GetOTResponsablesByOrdenTrabajo
{
    public class GetOTResponsablesByOrdenTrabajoQueryHandler
        : IRequestHandler<GetOTResponsablesByOrdenTrabajoQuery, List<OTResponsableResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetOTResponsablesByOrdenTrabajoQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<OTResponsableResponse>> Handle(
            GetOTResponsablesByOrdenTrabajoQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant();

            return await _uow.Mantenimiento.Transacciones.OTResponsables.Query()
                .AsNoTracking()
                .Where(o => o.PlantaCode == plantaCode && o.OrdenTrabajoCode == ordenCode)
                .OrderBy(o => o.FechaProceso)
                .Select(o => new OTResponsableResponse
                {
                    PlantaCode = o.PlantaCode,
                    OrdenTrabajoCode = o.OrdenTrabajoCode,
                    FechaProceso = o.FechaProceso,
                    TrabajadorCode = o.TrabajadorCode,
                    TiempoProceso = o.TiempoProceso,
                    Basico = o.Basico,
                    CostoTotal = o.CostoTotal,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    RowVersion = o.RowVersion
                })
                .ToListAsync(cancellationToken);
        }
    }
}
