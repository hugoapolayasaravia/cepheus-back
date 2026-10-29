using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.GetNotasCotizacionPlantillaPaginated
{
    public class GetNotasCotizacionPlantillaPaginatedQueryHandler
        : IRequestHandler<GetNotasCotizacionPlantillaPaginatedQuery, PagedResult<NotaCotizacionPlantillaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetNotasCotizacionPlantillaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<NotaCotizacionPlantillaResponse>> Handle(
            GetNotasCotizacionPlantillaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.NotasCotizacionPlantilla.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.NegocioCode))
            {
                query = query.Where(t => t.NegocioCode == request.NegocioCode);
            }

            if (request.Option.HasValue)
            {
                query = query.Where(t => t.Option == request.Option.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t => t.Description.ToLower().Contains(search));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.NegocioCode).ThenBy(t => t.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(t => new NotaCotizacionPlantillaResponse
            {
                NegocioCode = t.NegocioCode,
                Code = t.Code,
                Description = t.Description,
                Option = t.Option,
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
