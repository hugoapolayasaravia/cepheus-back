using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.GetOTRMaterialesByOrdenTrabajo
{
    public class GetOTRMaterialesByOrdenTrabajoQueryHandler
        : IRequestHandler<GetOTRMaterialesByOrdenTrabajoQuery, List<OTRMaterialResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetOTRMaterialesByOrdenTrabajoQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<OTRMaterialResponse>> Handle(
            GetOTRMaterialesByOrdenTrabajoQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant();

            return await _uow.Mantenimiento.Transacciones.OTRMateriales.Query()
                .AsNoTracking()
                .Where(o => o.PlantaCode == plantaCode && o.OrdenTrabajoCode == ordenCode)
                .OrderBy(o => o.FechaProceso)
                .Select(o => new OTRMaterialResponse
                {
                    PlantaCode = o.PlantaCode,
                    OrdenTrabajoCode = o.OrdenTrabajoCode,
                    FechaProceso = o.FechaProceso,
                    ArticuloCode = o.ArticuloCode,
                    Cantidad = o.Cantidad,
                    CostoUnitario = o.CostoUnitario,
                    CostoTotal = o.CostoTotal,
                    EstadoCode = o.EstadoCode,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    RowVersion = o.RowVersion
                })
                .ToListAsync(cancellationToken);
        }
    }
}
