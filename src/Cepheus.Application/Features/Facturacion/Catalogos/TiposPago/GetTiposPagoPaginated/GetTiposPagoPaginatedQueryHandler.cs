using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.GetTiposPagoPaginated
{
    public class GetTiposPagoPaginatedQueryHandler : IRequestHandler<GetTiposPagoPaginatedQuery, PagedResult<TipoPagoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTiposPagoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<TipoPagoResponse>> Handle(
            GetTiposPagoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.TiposPago.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t => t.Code.ToLower().Contains(search) || t.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            // Por defecto, el orden del legacy (Orden_tpa) y luego el código.
            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.SortOrder).ThenBy(t => t.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            return await sorted.Select(TipoPagoProjection.ToResponse)
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
