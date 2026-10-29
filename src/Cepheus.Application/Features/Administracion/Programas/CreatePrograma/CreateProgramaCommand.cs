using Cepheus.Application.Features.Administracion.Programas.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Programas.CreatePrograma
{
    public record CreateProgramaCommand(
        int SubmoduloId,
        string Code,
        string Name,
        string? Icon,
        string? Tooltip,
        string? Route,
        int DisplayOrder
    ) : IRequest<ProgramaResponse>;


}
