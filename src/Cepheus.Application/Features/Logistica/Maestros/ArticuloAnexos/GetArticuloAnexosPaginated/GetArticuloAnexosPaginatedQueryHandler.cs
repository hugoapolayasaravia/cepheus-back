using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.GetArticuloAnexosPaginated
{
    public class GetArticuloAnexosPaginatedQueryHandler
        : IRequestHandler<GetArticuloAnexosPaginatedQuery, PagedResult<ArticuloAnexoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloAnexosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ArticuloAnexoResponse>> Handle(
            GetArticuloAnexosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.ArticuloAnexos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SubCentroCostoCode))
            {
                var scc = request.SubCentroCostoCode.Trim().ToUpper();
                query = query.Where(a => a.SubCentroCostoCode == scc);
            }

            if (!string.IsNullOrWhiteSpace(request.ArticuloCode))
            {
                var art = request.ArticuloCode.Trim().ToUpper();
                query = query.Where(a => a.ArticuloCode == art);
            }

            if (!string.IsNullOrWhiteSpace(request.AccountingAccountCode))
            {
                var cta = request.AccountingAccountCode.Trim().ToUpper();
                query = query.Where(a => a.AccountingAccountCode == cta);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.SubCentroCostoCode).ThenBy(a => a.ArticuloCode)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<ArticuloAnexoResponse>
            {
                Items = paged.Items.Select(ArticuloAnexoMapper.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
