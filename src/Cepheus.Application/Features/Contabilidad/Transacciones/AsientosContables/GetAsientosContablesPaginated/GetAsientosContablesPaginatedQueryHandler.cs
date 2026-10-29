using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.GetAsientosContablesPaginated
{
    public class GetAsientosContablesPaginatedQueryHandler
        : IRequestHandler<GetAsientosContablesPaginatedQuery, PagedResult<AsientoContableListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAsientosContablesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<AsientoContableListItemResponse>> Handle(
            GetAsientosContablesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Transacciones.AsientosContables.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Anio))
            {
                var anio = request.Anio.Trim();
                query = query.Where(a => a.Anio == anio);
            }

            if (!string.IsNullOrWhiteSpace(request.Mes))
            {
                var mes = request.Mes.Trim().PadLeft(2, '0');
                query = query.Where(a => a.Mes == mes);
            }

            if (!string.IsNullOrWhiteSpace(request.TipoAsientoCode))
            {
                var tipo = request.TipoAsientoCode.Trim().ToUpper();
                query = query.Where(a => a.TipoAsientoCode == tipo);
            }

            if (request.Estado.HasValue)
            {
                query = query.Where(a => a.Estado == request.Estado.Value);
            }

            if (request.Origen.HasValue)
            {
                query = query.Where(a => a.Origen == request.Origen.Value);
            }

            if (request.FechaDesde.HasValue)
            {
                var desde = request.FechaDesde.Value.Date;
                query = query.Where(a => a.FechaProceso >= desde);
            }

            if (request.FechaHasta.HasValue)
            {
                var hasta = request.FechaHasta.Value.Date;
                query = query.Where(a => a.FechaProceso <= hasta);
            }

            if (!string.IsNullOrWhiteSpace(request.NumeroDocumento))
            {
                var documento = request.NumeroDocumento.Trim();
                query = query.Where(a => a.Detalles.Any(d => d.NumeroDocumento == documento));
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(a => a.Descripcion.Contains(search));
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(a => a.Anio).ThenByDescending(a => a.Mes)
                    .ThenBy(a => a.TipoAsientoCode).ThenByDescending(a => a.Numero)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<AsientoContableListItemResponse>
            {
                Items = paged.Items.Select(AsientoContableMapper.MapListItem).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
