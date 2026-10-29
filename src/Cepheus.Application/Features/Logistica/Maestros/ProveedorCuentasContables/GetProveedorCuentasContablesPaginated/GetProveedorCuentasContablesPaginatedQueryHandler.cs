using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.GetProveedorCuentasContablesPaginated
{
    public class GetProveedorCuentasContablesPaginatedQueryHandler
        : IRequestHandler<GetProveedorCuentasContablesPaginatedQuery, PagedResult<ProveedorCuentaContableResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorCuentasContablesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ProveedorCuentaContableResponse>> Handle(
            GetProveedorCuentasContablesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.ProveedorCuentasContables.Query()
                .AsNoTracking()
                .Include(c => c.Proveedor);

            IQueryable<Cepheus.Domain.Logistica.Maestros.ProveedorCuentaContable> filtered = query;

            if (!string.IsNullOrWhiteSpace(request.ProveedorCode))
            {
                var prv = request.ProveedorCode.Trim().ToUpper();
                filtered = filtered.Where(c => c.ProveedorCode == prv);
            }

            if (!string.IsNullOrWhiteSpace(request.MonedaCode))
            {
                var mon = request.MonedaCode.Trim().ToUpper();
                filtered = filtered.Where(c => c.MonedaCode == mon);
            }

            if (!string.IsNullOrWhiteSpace(request.AccountingAccountCode))
            {
                var cta = request.AccountingAccountCode.Trim().ToUpper();
                filtered = filtered.Where(c => c.AccountingAccountCode == cta);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? filtered.OrderBy(c => c.ProveedorCode).ThenBy(c => c.MonedaCode)
                : filtered.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<ProveedorCuentaContableResponse>
            {
                Items = paged.Items.Select(ProveedorCuentaContableMapper.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
