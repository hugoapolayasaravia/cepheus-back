using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.GetLugaresEnvioPaginated
{
    public class GetLugaresEnvioPaginatedQueryHandler
        : IRequestHandler<GetLugaresEnvioPaginatedQuery, PagedResult<LugarEnvioResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetLugaresEnvioPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<LugarEnvioResponse>> Handle(
            GetLugaresEnvioPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.LugaresEnvio.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(l =>
                    l.Code.ToLower().Contains(search) ||
                    l.Name.ToLower().Contains(search) ||
                    (l.Address != null && l.Address.ToLower().Contains(search)));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(l => l.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(l => l.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(l => new LugarEnvioResponse
            {
                Code = l.Code,
                Name = l.Name,
                Address = l.Address,
                IsActive = l.IsActive,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt,
                CreatedBy = l.CreatedBy,
                UpdatedBy = l.UpdatedBy,
                RowVersion = l.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}