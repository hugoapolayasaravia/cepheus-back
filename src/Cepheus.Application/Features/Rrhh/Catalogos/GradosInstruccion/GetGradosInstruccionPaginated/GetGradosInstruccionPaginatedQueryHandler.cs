using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.GetGradosInstruccionPaginated
{
    public class GetGradosInstruccionPaginatedQueryHandler
        : IRequestHandler<GetGradosInstruccionPaginatedQuery, PagedResult<GradoInstruccionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetGradosInstruccionPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<GradoInstruccionResponse>> Handle(
            GetGradosInstruccionPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Rrhh.Catalogos.GradosInstruccion.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(g =>
                    g.Code.ToLower().Contains(search) ||
                    g.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(g => g.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(g => g.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(g => new GradoInstruccionResponse
            {
                Code = g.Code,
                Name = g.Name,
                IsActive = g.IsActive,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedBy = g.CreatedBy,
                UpdatedBy = g.UpdatedBy,
                RowVersion = g.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}