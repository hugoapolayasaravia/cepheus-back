using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.Common;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.CreateTipoCuentaContable;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.GetTiposCuentaContablePaginated
{
    public class GetTiposCuentaContablePaginatedQueryHandler
        : IRequestHandler<GetTiposCuentaContablePaginatedQuery, PagedResult<TipoCuentaContableResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTiposCuentaContablePaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<TipoCuentaContableResponse>> Handle(
            GetTiposCuentaContablePaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Catalogos.TiposCuentaContable.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search));
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

            return new PagedResult<TipoCuentaContableResponse>
            {
                Items = paged.Items.Select(CreateTipoCuentaContableCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
