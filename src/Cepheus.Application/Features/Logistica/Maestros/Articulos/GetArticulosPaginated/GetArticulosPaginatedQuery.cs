using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Articulos.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.GetArticulosPaginated
{
    public class GetArticulosPaginatedQuery : PagedRequest, IRequest<PagedResult<ArticuloResponse>>
    {
        public string? Search { get; set; }
        public string? SubFamiliaCode { get; set; }
        public string? TipoArticuloCode { get; set; }
        public AbcClass? AbcClass { get; set; }
        public bool? IsActive { get; set; }
    }
}
