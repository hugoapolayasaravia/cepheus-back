using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresPorCombinacion
{
    public class GetAprobadoresPorCombinacionQueryHandler
        : IRequestHandler<GetAprobadoresPorCombinacionQuery, List<AprobadorAsignadoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAprobadoresPorCombinacionQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<AprobadorAsignadoResponse>> Handle(
            GetAprobadoresPorCombinacionQuery request, CancellationToken cancellationToken)
        {
            var nivel = request.NivelCode.Trim().ToUpperInvariant();
            var trans = request.TipoTransaccionCode.Trim().ToUpperInvariant();
            var une = request.UnidadNegocioCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();

            return await _uow.Logistica.Catalogos.AprobadoresAsignados.Query()
                .AsNoTracking()
                .Where(a => a.NivelCode == nivel && a.TipoTransaccionCode == trans &&
                            a.UnidadNegocioCode == une && a.MonedaCode == mon && a.IsActive)
                .OrderBy(a => a.TrabajadorCode)
                .Select(a => new AprobadorAsignadoResponse
                {
                    NivelCode = a.NivelCode,
                    TipoTransaccionCode = a.TipoTransaccionCode,
                    UnidadNegocioCode = a.UnidadNegocioCode,
                    MonedaCode = a.MonedaCode,
                    TrabajadorCode = a.TrabajadorCode,
                    SuplenteTrabajadorCode = a.SuplenteTrabajadorCode,
                    SuperiorTrabajadorCode = a.SuperiorTrabajadorCode,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    CreatedBy = a.CreatedBy,
                    UpdatedBy = a.UpdatedBy,
                    RowVersion = a.RowVersion
                })
                .ToListAsync(cancellationToken);
        }
    }
}
