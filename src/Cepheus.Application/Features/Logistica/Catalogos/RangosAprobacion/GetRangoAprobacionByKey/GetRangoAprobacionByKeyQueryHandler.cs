using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.GetRangoAprobacionByKey
{
    public class GetRangoAprobacionByKeyQueryHandler
        : IRequestHandler<GetRangoAprobacionByKeyQuery, RangoAprobacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetRangoAprobacionByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RangoAprobacionResponse> Handle(
            GetRangoAprobacionByKeyQuery request, CancellationToken cancellationToken)
        {
            var nivel = request.NivelCode.Trim().ToUpperInvariant();
            var trans = request.TipoTransaccionCode.Trim().ToUpperInvariant();
            var une = request.UnidadNegocioCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();

            var rango = await _uow.Logistica.Catalogos.RangosAprobacion.Query()
                .AsNoTracking()
                .Where(r => r.NivelCode == nivel && r.TipoTransaccionCode == trans &&
                            r.UnidadNegocioCode == une && r.MonedaCode == mon)
                .Select(r => new RangoAprobacionResponse
                {
                    NivelCode = r.NivelCode,
                    TipoTransaccionCode = r.TipoTransaccionCode,
                    UnidadNegocioCode = r.UnidadNegocioCode,
                    MonedaCode = r.MonedaCode,
                    ImporteMinimo = r.ImporteMinimo,
                    ImporteMaximo = r.ImporteMaximo,
                    ImporteAcumuladoDiario = r.ImporteAcumuladoDiario,
                    ImporteAcumuladoMensual = r.ImporteAcumuladoMensual,
                    PorcentajeTotal = r.PorcentajeTotal,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    CreatedBy = r.CreatedBy,
                    UpdatedBy = r.UpdatedBy,
                    RowVersion = r.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (rango is null)
            {
                throw new KeyNotFoundException($"Rango de aprobación {nivel}/{trans}/{une}/{mon} no encontrado.");
            }

            return rango;
        }
    }
}
