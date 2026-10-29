using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.GetNacionalidadesPaginated
{
    public class GetNacionalidadesPaginatedQuery : PagedRequest, IRequest<PagedResult<NacionalidadResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}