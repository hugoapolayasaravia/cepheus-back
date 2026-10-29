using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.GetProveedoresPaginated
{
    public class GetProveedoresPaginatedQueryHandler
        : IRequestHandler<GetProveedoresPaginatedQuery, PagedResult<ProveedorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedoresPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ProveedorResponse>> Handle(
            GetProveedoresPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.Proveedores.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(p =>
                    p.Code.ToLower().Contains(search) ||
                    p.DocumentNumber.ToLower().Contains(search) ||
                    p.LegalName.ToLower().Contains(search) ||
                    (p.TradeName != null && p.TradeName.ToLower().Contains(search)));
            }

            if (request.ProviderType.HasValue)
            {
                query = query.Where(p => p.ProviderType == request.ProviderType.Value);
            }

            if (request.Origin.HasValue)
            {
                query = query.Where(p => p.Origin == request.Origin.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(p => new ProveedorResponse
            {
                Code = p.Code,
                DocumentTypeCode = p.DocumentTypeCode,
                DocumentNumber = p.DocumentNumber,
                LegalName = p.LegalName,
                TradeName = p.TradeName,
                ProviderType = p.ProviderType,
                Origin = p.Origin,
                SunatCondition = p.SunatCondition,
                SunatStatus = p.SunatStatus,
                Observations = p.Observations,
                IsActive = p.IsActive,
                DeactivatedAt = p.DeactivatedAt,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy,
                RowVersion = p.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
