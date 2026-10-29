using Cepheus.Application.Features.Administracion.Modulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Modulos.CreateModulo
{
    public record CreateModuloCommand(
        string Code,
        string Name,
        string? Icon,
        string? Tooltip,
        int DisplayOrder
    ) : IRequest<ModuloResponse>;


}
