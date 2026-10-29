using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.CreateAtributoConcreto;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.GetAtributosConcretoPaginated
{
    public class GetAtributosConcretoPaginatedQueryHandler
        : IRequestHandler<GetAtributosConcretoPaginatedQuery, PagedResult<AtributoConcretoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAtributosConcretoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<AtributoConcretoResponse>> Handle(
            GetAtributosConcretoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.AtributosConcreto.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.AttributeType) &&
                System.Enum.TryParse<TipoAtributoConcreto>(request.AttributeType.Trim(), true, out var attributeType) &&
                System.Enum.IsDefined(attributeType))
            {
                query = query.Where(x => x.AttributeType == attributeType);
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

            return new PagedResult<AtributoConcretoResponse>
            {
                Items = paged.Items.Select(CreateAtributoConcretoCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
