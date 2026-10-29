using Cepheus.Application.Features.Administracion.Submodulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Submodulos.CreateSubmodulo
{
    public record CreateSubmoduloCommand(
        int ModuloId,
        string Code,
        string Name,
        string? Icon,
        string? Tooltip,
        int DisplayOrder
    ) : IRequest<SubmoduloResponse>;


}
