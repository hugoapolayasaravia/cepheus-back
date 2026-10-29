using Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.CreateUsuarioPlanta
{
    public record CreateUsuarioPlantaCommand(
        int UserId,
        string PlantaCode
    ) : IRequest<UsuarioPlantaResponse>;
}
