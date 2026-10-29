using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.GetModalidadesFormativasPaginated
{
    public class GetModalidadesFormativasPaginatedQuery : PagedRequest, IRequest<PagedResult<ModalidadFormativaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}