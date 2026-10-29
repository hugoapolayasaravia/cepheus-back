// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GetImportacionesPaginated/GetImportacionesPaginatedQueryHandler.cs
using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionesPaginated
{
    public class GetImportacionesPaginatedQueryHandler
        : IRequestHandler<GetImportacionesPaginatedQuery, PagedResult<ImportacionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetImportacionesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ImportacionResponse>> Handle(
            GetImportacionesPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Importacion> query = _uow.Logistica.Transacciones.Importaciones.Query()
                .AsNoTracking();

            if (!IsAll(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode!.Trim().ToUpper();
                query = query.Where(i => i.PlantaCode == plantaCode);
            }

            if (!IsAll(request.Code))
            {
                // Búsqueda exacta por código: ignora el rango de fechas (regla del SP legacy).
                var code = request.Code!.Trim().ToUpper();
                query = query.Where(i => i.Code == code);
            }
            else
            {
                if (request.FechaInicio.HasValue)
                {
                    var desde = request.FechaInicio.Value.Date;
                    query = query.Where(i => i.CreatedAt >= desde);
                }

                if (request.FechaFin.HasValue)
                {
                    // Incluye todo el día final.
                    var hastaExclusivo = request.FechaFin.Value.Date.AddDays(1);
                    query = query.Where(i => i.CreatedAt < hastaExclusivo);
                }
            }

            if (!IsAll(request.Estado)
                && System.Enum.TryParse<EstadoImportacion>(request.Estado, true, out var estado))
            {
                query = query.Where(i => i.Estado == estado);
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(i => i.CreatedAt).ThenByDescending(i => i.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var pagedEntities = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<ImportacionResponse>
            {
                Items = pagedEntities.Items.Select(ImportacionMapper.Map).ToList(),
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }

        /// <summary>null, vacío o el centinela legacy 'T' = sin filtro.</summary>
        private static bool IsAll(string? value)
            => string.IsNullOrWhiteSpace(value)
               || string.Equals(value.Trim(), "T", StringComparison.OrdinalIgnoreCase);
    }
}
