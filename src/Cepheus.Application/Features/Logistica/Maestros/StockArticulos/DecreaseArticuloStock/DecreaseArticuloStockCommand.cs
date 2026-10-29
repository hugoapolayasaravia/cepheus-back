using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.DecreaseArticuloStock
{
    /// <summary>
    /// Reduce el stock (ej. vale de salida, consumo). Rechaza la operación si
    /// el resultado quedaría negativo. Reservado para el futuro módulo de
    /// Transacciones.
    /// </summary>
    public record DecreaseArticuloStockCommand(
        string PlantaCode,
        string ArticuloCode,
        decimal Quantity
    ) : IRequest<ArticuloStockResponse>;
}
