using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.CreateCobrador;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.GetCobradoresPaginated
{
    public class GetCobradoresPaginatedQueryHandler
        : IRequestHandler<GetCobradoresPaginatedQuery, PagedResult<CobradorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCobradoresPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CobradorResponse>> Handle(
            GetCobradoresPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Maestros.Cobradores.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search) ||
                    (x.Email != null && x.Email.ToLower().Contains(search)));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            // Se pagina la entidad y se mapea en memoria (mismo criterio que OrdenTrabajo).
            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<CobradorResponse>
            {
                Items = paged.Items.Select(CreateCobradorCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
