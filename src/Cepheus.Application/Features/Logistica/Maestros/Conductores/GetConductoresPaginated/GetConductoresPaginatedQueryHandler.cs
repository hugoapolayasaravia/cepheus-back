using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.GetConductoresPaginated
{
    public class GetConductoresPaginatedQueryHandler
        : IRequestHandler<GetConductoresPaginatedQuery, PagedResult<ConductorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetConductoresPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ConductorResponse>> Handle(
            GetConductoresPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.Conductores.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.DocumentNumber.ToLower().Contains(search) ||
                    c.FirstName.ToLower().Contains(search) ||
                    c.LastName.ToLower().Contains(search) ||
                    c.DriverLicenseNumber.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(c => c.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(c => new ConductorResponse
            {
                Code = c.Code,
                DocumentTypeCode = c.DocumentTypeCode,
                DocumentNumber = c.DocumentNumber,
                FirstName = c.FirstName,
                LastName = c.LastName,
                DriverLicenseNumber = c.DriverLicenseNumber,
                LicenseCategory = c.LicenseCategory,
                Phone = c.Phone,
                Email = c.Email,
                Observations = c.Observations,
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
