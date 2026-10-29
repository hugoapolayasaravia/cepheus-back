using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Programas.ExportProgramas
{
    public class ExportProgramasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public int? SubmoduloId { get; set; }
    }
}
