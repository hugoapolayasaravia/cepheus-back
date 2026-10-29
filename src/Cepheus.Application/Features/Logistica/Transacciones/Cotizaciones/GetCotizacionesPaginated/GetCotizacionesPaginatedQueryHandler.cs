using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.GetCotizacionesPaginated
{
    public class GetCotizacionesPaginatedQueryHandler
        : IRequestHandler<GetCotizacionesPaginatedQuery, PagedResult<CotizacionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCotizacionesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CotizacionResponse>> Handle(
            GetCotizacionesPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Cotizacion> query = _uow.Logistica.Transacciones.Cotizaciones.Query()
                .AsNoTracking()
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Origenes)
                .Include(c => c.Proveedores)
                    .ThenInclude(p => p.Detalles);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();

                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.Observaciones.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();

                query = query.Where(c =>
                    c.PlantaCode == plantaCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado)
                && System.Enum.TryParse<EstadoCotizacion>(
                    request.Estado,
                    true,
                    out var estado))
            {
                query = query.Where(c =>
                    c.Estado == estado);
            }

            if (request.FechaLimiteDesde.HasValue)
            {
                query = query.Where(c =>
                    c.FechaLimite >= request.FechaLimiteDesde.Value);
            }

            if (request.FechaLimiteHasta.HasValue)
            {
                query = query.Where(c =>
                    c.FechaLimite <= request.FechaLimiteHasta.Value);
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.ApplySort(request.SortBy, sortDesc);

            var pagedEntities = await sortedQuery.ToPagedResultAsync(
                pageNumber,
                pageSize,
                cancellationToken);

            return new PagedResult<CotizacionResponse>
            {
                Items = pagedEntities.Items
                    .Select(CotizacionMapper.Map)
                    .ToList(),

                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}
