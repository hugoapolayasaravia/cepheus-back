using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.ExportRoles
{
    public class ExportRolesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
