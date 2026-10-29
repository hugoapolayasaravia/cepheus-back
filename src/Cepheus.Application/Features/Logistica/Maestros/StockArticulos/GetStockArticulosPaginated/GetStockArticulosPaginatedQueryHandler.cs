using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.GetStockArticulosPaginated
{
    public class GetStockArticulosPaginatedQueryHandler
        : IRequestHandler<GetStockArticulosPaginatedQuery, PagedResult<ArticuloStockResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetStockArticulosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ArticuloStockResponse>> Handle(
            GetStockArticulosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.StockArticulos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();
                query = query.Where(s => s.PlantaCode == plantaCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ArticuloCode))
            {
                var articuloCode = request.ArticuloCode.Trim().ToUpper();
                query = query.Where(s => s.ArticuloCode == articuloCode);
            }

            if (request.BelowMinimum == true)
            {
                query = query.Where(s => s.Quantity <= s.MinStock);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(s => s.PlantaCode).ThenBy(s => s.ArticuloCode)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(s => new ArticuloStockResponse
            {
                PlantaCode = s.PlantaCode,
                ArticuloCode = s.ArticuloCode,
                Quantity = s.Quantity,
                UnitCost = s.UnitCost,
                UnitCostUsd = s.UnitCostUsd,
                AverageCost = s.AverageCost,
                MinStock = s.MinStock,
                MaxStock = s.MaxStock,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CreatedBy = s.CreatedBy,
                UpdatedBy = s.UpdatedBy,
                RowVersion = s.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
