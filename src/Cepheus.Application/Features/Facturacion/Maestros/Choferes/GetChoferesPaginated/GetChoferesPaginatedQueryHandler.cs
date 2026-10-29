using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.GetChoferesPaginated
{
    public class GetChoferesPaginatedQueryHandler
        : IRequestHandler<GetChoferesPaginatedQuery, PagedResult<ChoferResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetChoferesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ChoferResponse>> Handle(
            GetChoferesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Maestros.Choferes.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.FullName.ToLower().Contains(search) ||
                    c.DriverLicenseNumber.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.TransportistaCode))
            {
                var transportistaCode = request.TransportistaCode.Trim().ToUpper();
                query = query.Where(c => c.TransportistaCode == transportistaCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(c => c.TransportistaCode).ThenBy(c => c.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(c => new ChoferResponse
            {
                TransportistaCode = c.TransportistaCode,
                Code = c.Code,
                FullName = c.FullName,
                DriverLicenseNumber = c.DriverLicenseNumber,
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
