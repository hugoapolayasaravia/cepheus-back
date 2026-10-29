using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.GetTransportistasPaginated
{
    public class GetTransportistasPaginatedQueryHandler
        : IRequestHandler<GetTransportistasPaginatedQuery, PagedResult<TransportistaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTransportistasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<TransportistaResponse>> Handle(
            GetTransportistasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Maestros.Transportistas.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    t.DocumentNumber.ToLower().Contains(search) ||
                    t.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(t => new TransportistaResponse
            {
                Code = t.Code,
                DocumentTypeCode = t.DocumentTypeCode,
                DocumentNumber = t.DocumentNumber,
                Name = t.Name,
                Address = t.Address,
                UbigeoCode = t.UbigeoCode,
                Phone = t.Phone,
                Email = t.Email,
                MtcInternalCode = t.MtcInternalCode,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
