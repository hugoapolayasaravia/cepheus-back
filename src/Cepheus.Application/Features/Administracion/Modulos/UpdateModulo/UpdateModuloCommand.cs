using Cepheus.Application.Features.Administracion.Modulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Modulos.UpdateModulo
{
    public record UpdateModuloCommand(
        int Id,
        string Code,
        string Name,
        string? Icon,
        string? Tooltip,
        int DisplayOrder,
        byte[] RowVersion
    ) : IRequest<ModuloResponse>;

}
