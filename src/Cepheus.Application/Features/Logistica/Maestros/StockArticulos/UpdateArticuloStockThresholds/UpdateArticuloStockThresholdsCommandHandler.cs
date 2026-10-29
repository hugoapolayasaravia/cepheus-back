using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.UpdateArticuloStockThresholds
{
    public class UpdateArticuloStockThresholdsCommandHandler
        : IRequestHandler<UpdateArticuloStockThresholdsCommand, ArticuloStockResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateArticuloStockThresholdsCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloStockResponse> Handle(UpdateArticuloStockThresholdsCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Maestros.StockArticulos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.PlantaCode == request.PlantaCode && s.ArticuloCode == request.ArticuloCode, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException(
                    $"No hay registro de stock para artículo {request.ArticuloCode} en planta {request.PlantaCode}.");
            }

            var stock = new Domain.Logistica.Maestros.ArticuloStock
            {
                PlantaCode = request.PlantaCode,
                ArticuloCode = request.ArticuloCode,

                // Quantity NO se toca acá — se preserva el valor actual.
                Quantity = current.Quantity,

                MinStock = request.MinStock,
                MaxStock = request.MaxStock,
                UnitCost = request.UnitCost,
                UnitCostUsd = request.UnitCostUsd,
                AverageCost = request.AverageCost,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Maestros.StockArticulos.Update(stock);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El stock fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateArticuloStock.CreateArticuloStockCommandHandler.Map(stock);
        }
    }
}
