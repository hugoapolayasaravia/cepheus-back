using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.IncreaseArticuloStock
{
    /// <summary>
    /// Incrementa el stock (ej. recepción de una Orden de Compra). Reservado
    /// para ser invocado desde el futuro módulo de Transacciones — acá se
    /// deja el mecanismo listo y funcional.
    /// </summary>
    public record IncreaseArticuloStockCommand(
        string PlantaCode,
        string ArticuloCode,
        decimal Quantity
    ) : IRequest<ArticuloStockResponse>;
}
