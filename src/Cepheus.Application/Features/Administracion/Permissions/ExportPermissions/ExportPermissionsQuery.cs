using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Permissions.ExportPermissions
{
    public class ExportPermissionsQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public int? ProgramaId { get; set; }
    }
}
