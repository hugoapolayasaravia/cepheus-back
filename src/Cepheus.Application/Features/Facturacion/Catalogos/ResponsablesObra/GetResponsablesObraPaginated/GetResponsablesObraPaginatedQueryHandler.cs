using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.GetResponsablesObraPaginated
{
    public class GetResponsablesObraPaginatedQueryHandler
        : IRequestHandler<GetResponsablesObraPaginatedQuery, PagedResult<ResponsableObraResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetResponsablesObraPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ResponsableObraResponse>> Handle(
            GetResponsablesObraPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.ResponsablesObra.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.ClienteCode))
            {
                query = query.Where(r => r.ClienteCode == request.ClienteCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ObraCode))
            {
                query = query.Where(r => r.ObraCode == request.ObraCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(r =>
                    r.Name.Contains(search) ||
                    r.Position.Contains(search) ||
                    r.Phone.Contains(search));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(r => r.ClienteCode).ThenBy(r => r.ObraCode).ThenBy(r => r.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(r => new ResponsableObraResponse
            {
                ClienteCode = r.ClienteCode,
                ObraCode = r.ObraCode,
                Code = r.Code,
                Name = r.Name,
                Phone = r.Phone,
                Position = r.Position,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                CreatedBy = r.CreatedBy,
                UpdatedBy = r.UpdatedBy,
                RowVersion = r.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
