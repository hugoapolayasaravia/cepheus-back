using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.GetMotivosChequeRebotePaginated
{
    public class GetMotivosChequeRebotePaginatedQueryHandler
        : IRequestHandler<GetMotivosChequeRebotePaginatedQuery, PagedResult<MotivoChequeReboteResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetMotivosChequeRebotePaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<MotivoChequeReboteResponse>> Handle(
            GetMotivosChequeRebotePaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.MotivosChequeRebote.Query().AsNoTracking();

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

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            return await sorted.Select(MotivoChequeReboteProjection.ToResponse)
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
