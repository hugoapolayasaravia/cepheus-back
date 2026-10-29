using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.GetArticuloAnexosPaginated
{
    public class GetArticuloAnexosPaginatedQuery : PagedRequest, IRequest<PagedResult<ArticuloAnexoResponse>>
    {
        public string? SubCentroCostoCode { get; set; }
        public string? ArticuloCode { get; set; }
        public string? AccountingAccountCode { get; set; }
    }
}
