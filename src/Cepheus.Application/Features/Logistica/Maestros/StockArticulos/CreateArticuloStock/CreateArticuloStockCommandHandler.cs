using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.CreateArticuloStock
{
    public class CreateArticuloStockCommandHandler : IRequestHandler<CreateArticuloStockCommand, ArticuloStockResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IArticuloStockMovementService _stock;

        public CreateArticuloStockCommandHandler(IUnitOfWork uow, IArticuloStockMovementService stock)
        {
            _uow = uow;
            _stock = stock;
        }

        public async Task<ArticuloStockResponse> Handle(CreateArticuloStockCommand request, CancellationToken cancellationToken)
        {
            var stock = new ArticuloStock
            {
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                ArticuloCode = request.ArticuloCode.Trim().ToUpperInvariant(),
                Quantity = request.InitialQuantity,
                UnitCost = request.UnitCost,
                UnitCostUsd = request.UnitCostUsd,
                AverageCost = request.AverageCost,
                MinStock = request.MinStock,
                MaxStock = request.MaxStock
            };

            await _uow.Logistica.Maestros.StockArticulos.AddAsync(stock, cancellationToken);

            // El saldo inicial entra al Kardex como Apertura (misma transacción que el alta del stock).
            await _stock.RecordOpeningAsync(stock, DateTime.Today, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(stock);
        }

        internal static ArticuloStockResponse Map(ArticuloStock stock) => new()
        {
            PlantaCode = stock.PlantaCode,
            ArticuloCode = stock.ArticuloCode,
            Quantity = stock.Quantity,
            UnitCost = stock.UnitCost,
            UnitCostUsd = stock.UnitCostUsd,
            AverageCost = stock.AverageCost,
            MinStock = stock.MinStock,
            MaxStock = stock.MaxStock,
            CreatedAt = stock.CreatedAt,
            UpdatedAt = stock.UpdatedAt,
            RowVersion = stock.RowVersion
        };
    }
}
