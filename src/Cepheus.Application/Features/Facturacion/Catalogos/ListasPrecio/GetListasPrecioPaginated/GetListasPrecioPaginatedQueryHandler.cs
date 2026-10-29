using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.CreateListaPrecio;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.GetListasPrecioPaginated
{
    public class GetListasPrecioPaginatedQueryHandler
        : IRequestHandler<GetListasPrecioPaginatedQuery, PagedResult<ListaPrecioResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetListasPrecioPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ListaPrecioResponse>> Handle(
            GetListasPrecioPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.ListasPrecio.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.TipoProductoCode))
            {
                var tipo = request.TipoProductoCode.Trim().ToUpper();
                query = query.Where(x => x.TipoProductoCode == tipo);
            }

            if (!string.IsNullOrWhiteSpace(request.ProductoCode))
            {
                var code = request.ProductoCode.Trim().ToUpper();
                query = query.Where(x => x.ProductoCode == code);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            if (request.Vigente == true)
            {
                var now = DateTime.UtcNow;
                query = query.Where(x => x.FechaInicio <= now && (x.FechaFin == null || x.FechaFin >= now));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.FechaInicio)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<ListaPrecioResponse>
            {
                Items = paged.Items.Select(CreateListaPrecioCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
