using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.GetArticuloStock
{
    public class GetArticuloStockQueryHandler : IRequestHandler<GetArticuloStockQuery, ArticuloStockResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloStockQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloStockResponse> Handle(GetArticuloStockQuery request, CancellationToken cancellationToken)
        {
            var stock = await _uow.Logistica.Maestros.StockArticulos.Query()
                .AsNoTracking()
                .Where(s => s.PlantaCode == request.PlantaCode && s.ArticuloCode == request.ArticuloCode)
                .Select(s => new ArticuloStockResponse
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
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (stock is null)
            {
                throw new KeyNotFoundException(
                    $"No hay registro de stock para artículo {request.ArticuloCode} en planta {request.PlantaCode}.");
            }

            return stock;
        }
    }
}
