using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.GetArticuloStock
{
    public record GetArticuloStockQuery(string PlantaCode, string ArticuloCode) : IRequest<ArticuloStockResponse>;
}
