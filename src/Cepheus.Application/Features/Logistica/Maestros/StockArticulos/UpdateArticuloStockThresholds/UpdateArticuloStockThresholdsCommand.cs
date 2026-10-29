using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.UpdateArticuloStockThresholds
{
    /// <summary>
    /// Actualiza umbrales (min/max) y costos — NUNCA la cantidad (Quantity),
    /// que solo se mueve vía Increase/Decrease.
    /// </summary>
    public record UpdateArticuloStockThresholdsCommand(
        string PlantaCode,
        string ArticuloCode,
        decimal MinStock,
        decimal MaxStock,
        decimal UnitCost,
        decimal UnitCostUsd,
        decimal? AverageCost,
        byte[] RowVersion
    ) : IRequest<ArticuloStockResponse>;
}
