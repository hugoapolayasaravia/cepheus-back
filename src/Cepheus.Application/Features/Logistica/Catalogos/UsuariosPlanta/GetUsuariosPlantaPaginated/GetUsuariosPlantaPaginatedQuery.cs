using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.GetUsuariosPlantaPaginated
{
    public class GetUsuariosPlantaPaginatedQuery : PagedRequest, IRequest<PagedResult<UsuarioPlantaResponse>>
    {
        /// <summary>Busca por email o nombre del usuario, y por código o nombre de la planta.</summary>
        public string? Search { get; set; }
        public int? UserId { get; set; }
        public string? PlantaCode { get; set; }
    }
}
