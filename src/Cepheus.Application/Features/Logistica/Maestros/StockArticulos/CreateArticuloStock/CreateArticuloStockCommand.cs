using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.CreateArticuloStock
{
    /// <summary>
    /// Registra por primera vez el stock de un artículo en una planta (ej. al
    /// habilitar un artículo en una planta nueva, o carga inicial de
    /// inventario). No es para movimientos posteriores — eso es
    /// IncreaseStock/DecreaseStock.
    /// </summary>
    public record CreateArticuloStockCommand(
        string PlantaCode,
        string ArticuloCode,
        decimal InitialQuantity,
        decimal UnitCost,
        decimal UnitCostUsd,
        decimal? AverageCost,
        decimal MinStock,
        decimal MaxStock
    ) : IRequest<ArticuloStockResponse>;
}
