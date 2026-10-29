using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.GetStockArticulosPaginated
{
    public class GetStockArticulosPaginatedQuery : PagedRequest, IRequest<PagedResult<ArticuloStockResponse>>
    {
        public string? PlantaCode { get; set; }
        public string? ArticuloCode { get; set; }

        /// <summary>Si es true, solo trae registros con Quantity &lt;= MinStock (quiebre/próximo a quiebre).</summary>
        public bool? BelowMinimum { get; set; }
    }
}
