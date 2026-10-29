using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.ComprobantesPago.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.GetComprobantesPagoPaginated
{
    public class GetComprobantesPagoPaginatedQueryHandler
        : IRequestHandler<GetComprobantesPagoPaginatedQuery, PagedResult<ComprobantePagoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetComprobantesPagoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ComprobantePagoResponse>> Handle(
            GetComprobantesPagoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Comunes.ComprobantesPago.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(c => c.Name)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(c => new ComprobantePagoResponse
            {
                Code = c.Code,
                SunatCode = c.SunatCode,
                Name = c.Name,
                ShortName = c.ShortName,
                Description = c.Description,
                RequiresRuc = c.RequiresRuc,
                RequiresAddress = c.RequiresAddress,
                AffectsIgv = c.AffectsIgv,
                IsNonTaxable = c.IsNonTaxable,
                AffectsIncomeTax = c.AffectsIncomeTax,
                AffectsFonavi = c.AffectsFonavi,
                IsService = c.IsService,
                AffectsForeignIgv = c.AffectsForeignIgv,
                AvailableForPurchaseOrder = c.AvailableForPurchaseOrder,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                RowVersion = c.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
