using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.GetAnalisisVentasPaginated
{
    public class GetAnalisisVentasPaginatedQueryHandler
        : IRequestHandler<GetAnalisisVentasPaginatedQuery, PagedResult<AnalisisVentaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAnalisisVentasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<AnalisisVentaResponse>> Handle(
            GetAnalisisVentasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.AnalisisVentas.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(a =>
                    a.Code.ToLower().Contains(search) ||
                    a.Name.ToLower().Contains(search) ||
                    (a.ShortName != null && a.ShortName.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.SegmentoVentasCode))
            {
                var segmentoCode = request.SegmentoVentasCode.Trim().ToUpper();
                query = query.Where(a => a.SegmentoVentasCode == segmentoCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(a => new AnalisisVentaResponse
            {
                Code = a.Code,
                Name = a.Name,
                ShortName = a.ShortName,
                SegmentoVentasCode = a.SegmentoVentasCode,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                CreatedBy = a.CreatedBy,
                UpdatedBy = a.UpdatedBy,
                RowVersion = a.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
