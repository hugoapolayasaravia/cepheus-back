using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.CreateAnexo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.GetAnexosPaginated
{
    public class GetAnexosPaginatedQueryHandler
        : IRequestHandler<GetAnexosPaginatedQuery, PagedResult<AnexoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAnexosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<AnexoResponse>> Handle(
            GetAnexosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Maestros.Anexos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    (x.Name != null && x.Name.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.TipoAnexoCode))
            {
                var tipo = request.TipoAnexoCode.Trim();
                query = query.Where(x => x.TipoAnexoCode == tipo);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.TipoAnexoCode).ThenBy(x => x.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            // Se pagina la entidad y se mapea en memoria (mismo criterio que OrdenTrabajo).
            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<AnexoResponse>
            {
                Items = paged.Items.Select(CreateAnexoCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
