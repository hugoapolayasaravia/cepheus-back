using Cepheus.Application.Features.Administracion.Submodulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Submodulos.UpdateSubmodulo
{
    /// <summary>
    /// ModuloId NO se puede editar acá a propósito — mover un submódulo de módulo
    /// es una operación estructural distinta, no un simple update de campos.
    /// </summary>
    public record UpdateSubmoduloCommand(
        int Id,
        string Code,
        string Name,
        string? Icon,
        string? Tooltip,
        int DisplayOrder,
        byte[] RowVersion
    ) : IRequest<SubmoduloResponse>;


}
