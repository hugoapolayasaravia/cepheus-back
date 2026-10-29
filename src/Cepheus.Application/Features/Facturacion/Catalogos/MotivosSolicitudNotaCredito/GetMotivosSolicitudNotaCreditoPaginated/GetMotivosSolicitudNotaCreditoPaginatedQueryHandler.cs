using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.GetMotivosSolicitudNotaCreditoPaginated
{
    public class GetMotivosSolicitudNotaCreditoPaginatedQueryHandler
        : IRequestHandler<GetMotivosSolicitudNotaCreditoPaginatedQuery, PagedResult<MotivoSolicitudNotaCreditoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetMotivosSolicitudNotaCreditoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<MotivoSolicitudNotaCreditoResponse>> Handle(
            GetMotivosSolicitudNotaCreditoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    t.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.ParentCode))
            {
                var parent = request.ParentCode.Trim();
                query = query.Where(t => t.ParentCode == parent);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            if (request.OnlyCurrent == true)
            {
                var today = DateTime.UtcNow.Date;
                query = query.Where(t =>
                    (t.StartDate == null || t.StartDate <= today) &&
                    (t.EndDate == null || t.EndDate >= today));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(MotivoSolicitudNotaCreditoProjection.ToResponse);

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
