using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.GetRangosAprobacionPaginated
{
    public class GetRangosAprobacionPaginatedQueryHandler
        : IRequestHandler<GetRangosAprobacionPaginatedQuery, PagedResult<RangoAprobacionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetRangosAprobacionPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<RangoAprobacionResponse>> Handle(
            GetRangosAprobacionPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.RangosAprobacion.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.TipoTransaccionCode))
            {
                var t = request.TipoTransaccionCode.Trim().ToUpper();
                query = query.Where(r => r.TipoTransaccionCode == t);
            }

            if (!string.IsNullOrWhiteSpace(request.UnidadNegocioCode))
            {
                var u = request.UnidadNegocioCode.Trim().ToUpper();
                query = query.Where(r => r.UnidadNegocioCode == u);
            }

            if (!string.IsNullOrWhiteSpace(request.MonedaCode))
            {
                var m = request.MonedaCode.Trim().ToUpper();
                query = query.Where(r => r.MonedaCode == m);
            }

            if (!string.IsNullOrWhiteSpace(request.NivelCode))
            {
                var n = request.NivelCode.Trim().ToUpper();
                query = query.Where(r => r.NivelCode == n);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(r => r.TipoTransaccionCode).ThenBy(r => r.UnidadNegocioCode).ThenBy(r => r.NivelCode)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(r => new RangoAprobacionResponse
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
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
