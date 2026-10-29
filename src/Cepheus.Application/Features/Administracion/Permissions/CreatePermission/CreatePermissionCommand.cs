using Cepheus.Application.Features.Administracion.Permissions.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Permissions.CreatePermission
{
    /// <summary>
    /// Alta MANUAL de un permiso adicional para un Programa que ya existe
    /// (ej. "EXPORT_PDF" para el botón de exportar, "ANNUL" para anular).
    /// El CRUD básico (VIEW/CREATE/UPDATE/DELETE) ya se generó solo al crear
    /// el Programa — esto es para todo lo que no entra en ese estándar.
    /// </summary>
    public record CreatePermissionCommand(
        int ProgramaId,
        string Code,
        string Name
    ) : IRequest<PermissionResponse>;

}
