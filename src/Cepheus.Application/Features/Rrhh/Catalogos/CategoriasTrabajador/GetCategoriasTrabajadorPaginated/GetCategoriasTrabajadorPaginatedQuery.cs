using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.GetCategoriasTrabajadorPaginated
{
    public class GetCategoriasTrabajadorPaginatedQuery : PagedRequest, IRequest<PagedResult<CategoriaTrabajadorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}