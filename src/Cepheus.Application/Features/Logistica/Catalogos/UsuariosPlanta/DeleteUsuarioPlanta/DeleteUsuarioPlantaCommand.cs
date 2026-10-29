using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.DeleteUsuarioPlanta
{
    public record DeleteUsuarioPlantaCommand(int UserId, string PlantaCode) : IRequest;
}
