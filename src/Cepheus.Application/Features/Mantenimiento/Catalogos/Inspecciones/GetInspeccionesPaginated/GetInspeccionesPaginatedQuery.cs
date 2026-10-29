using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.GetInspeccionesPaginated
{
    public class GetInspeccionesPaginatedQuery : PagedRequest, IRequest<PagedResult<InspeccionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
