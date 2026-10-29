using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.GetUnidadesMedidaPaginated
{
    public class GetUnidadesMedidaPaginatedQueryHandler
        : IRequestHandler<GetUnidadesMedidaPaginatedQuery, PagedResult<UnidadMedidaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetUnidadesMedidaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<UnidadMedidaResponse>> Handle(
            GetUnidadesMedidaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.UnidadesMedida.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.Code.ToLower().Contains(search) ||
                    u.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(u => u.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(u => new UnidadMedidaResponse
            {
                Code = u.Code,
                Name = u.Name,
                IndDecimal = u.IndDecimal,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                CreatedBy = u.CreatedBy,
                UpdatedBy = u.UpdatedBy,
                RowVersion = u.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}