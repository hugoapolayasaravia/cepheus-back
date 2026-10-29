// Cepheus.Application/Features/Logistica/Transacciones/Guias/GetGuiasPaginated/GetGuiasPaginatedQueryHandler.cs
using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GetGuiasPaginated
{
    public class GetGuiasPaginatedQueryHandler
        : IRequestHandler<GetGuiasPaginatedQuery, PagedResult<GuiaListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetGuiasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<GuiaListItemResponse>> Handle(
            GetGuiasPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Guia> query = _uow.Logistica.Transacciones.Guias.Query()
                .AsNoTracking()
                .WithHeaderIncludes();

            var plantaCode = Filter(request.PlantaCode);
            if (plantaCode is not null)
            {
                query = query.Where(g => g.PlantaCode == plantaCode);
            }

            var guiaNumber = Filter(request.Guia);

            if (guiaNumber is not null)
            {
                // Legacy: con una guía específica solo se busca por su número.
                query = query.Where(g => g.Code == guiaNumber);
            }
            else
            {
                if (request.FechaInicio.HasValue)
                {
                    var desde = request.FechaInicio.Value.Date;
                    query = query.Where(g => g.FechaEmision >= desde);
                }

                if (request.FechaFin.HasValue)
                {
                    var hasta = request.FechaFin.Value.Date.AddDays(1);
                    query = query.Where(g => g.FechaEmision < hasta);
                }

                var serie = Filter(request.Serie);
                if (serie is not null)
                {
                    var prefix = serie + "-";
                    query = query.Where(g => g.Code.StartsWith(prefix));
                }

                var proveedorCode = Filter(request.ProveedorCode);
                if (proveedorCode is not null)
                {
                    query = query.Where(g => g.ProveedorCode == proveedorCode);
                }

                var estadoText = Filter(request.Estado);
                if (estadoText is not null
                    && System.Enum.TryParse<EstadoGuia>(estadoText, true, out var estado))
                {
                    query = query.Where(g => g.Estado == estado);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();

                query = query.Where(g =>
                    g.Code.ToLower().Contains(search) ||
                    g.Observaciones.ToLower().Contains(search) ||
                    g.Proveedor.LegalName.ToLower().Contains(search));
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            IQueryable<Guia> sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(g => g.CreatedAt).ThenByDescending(g => g.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var pagedEntities = await sortedQuery.ToPagedResultAsync(
                pageNumber,
                pageSize,
                cancellationToken);

            return new PagedResult<GuiaListItemResponse>
            {
                Items = pagedEntities.Items
                    .Select(GuiaMapper.ToListItem)
                    .ToList(),

                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }

        /// <summary>Vacío o "T" (todos, como en el legacy) => sin filtro.</summary>
        private static string? Filter(string? value)
            => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase)
                ? null
                : value.Trim().ToUpperInvariant();
    }
}
