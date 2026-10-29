using Cepheus.Application.Features.Administracion.Permissions.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Permissions.UpdatePermission
{
    /// <summary>
    /// ProgramaId NO se puede editar acá a propósito, mismo criterio que
    /// UpdateSubmodulo/UpdatePrograma.
    /// </summary>
    public record UpdatePermissionCommand(
        int Id,
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<PermissionResponse>;

}
